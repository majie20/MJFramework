using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace WXB
{
    [RequireComponent(typeof(SymbolText))]
    public class SymbolTextPlayAnim : MonoBehaviour
    {
        public struct PlayInfo
        {
            public int   index;
            public int   length;
            public float speed;
        }

        private static string REGEX_STR1 = @"<speed=([0-9\.]+)>";
        private static string REGEX_STR2 = @"</speed>";

        private SymbolText      _symbolText;
        private float           _delaySkipTime;
        private string          _text;
        private bool            _isCanSkip;
        private bool            _isRunning;
        private bool            _isSkip;
        private Action          _onComplete;
        private List<PlayInfo>  _playList;
        private Coroutine       _coroutine;
        private Queue<NodeBase> _nodeList;

        public bool IsRunning
        {
            get => _isRunning;
        }

        private void Awake()
        {
            _symbolText = GetComponent<SymbolText>();
            _playList = new List<PlayInfo>();
            _nodeList = new Queue<NodeBase>();

            //Play(
            //    "ddddddddddddd#u<speed=0.3>#cFF0000ff#d4ksksk\n#r#001<sprite n=unity w=40 h=40>ddd\ndd<hy t=dasda l=123 fs=30 ft=3 fc=#000000 ul=1 so=1></speed>#g#x100<speed=0.5>#d4dadad#hqqq[456]#h</speed><speed=0.1>dddddddddddd<face n=i501 w=50 h=50>ddddddddddddddddddddddd</speed>",
            //    true, 2f, () => Debug.Log("Œ“∞Æ√®√®£°"));
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void LateUpdate()
        {
            if (_isCanSkip && !_isSkip)
            {
                _delaySkipTime -= Time.deltaTime;

                if (_delaySkipTime <= 0f)
                {
                    _isSkip = true;
                }
            }
        }

        public bool Play(string text, bool isCanSkip, float delaySkipTime = 0, Action onComplete = null)
        {
            if (_isRunning)
            {
                return false;
            }

            _delaySkipTime = delaySkipTime;
            _isCanSkip = isCanSkip;
            _onComplete = onComplete;
            _playList.Clear();
            _nodeList.Clear();

            var cfg = _symbolText.CreateConfig();

            while (true)
            {
                var match1 = Regex.Match(text, REGEX_STR1);

                if (match1.Success)
                {
                    var playInfo = new PlayInfo {index = match1.Index, speed = float.Parse(match1.Groups[1].Value)};
                    text = $"{text[..match1.Index]}{text[(match1.Index + match1.Length)..]}";

                    var match2 = Regex.Match(text, REGEX_STR2);
                    playInfo.length = match2.Index - match1.Index;
                    text = $"{text[..match2.Index]}{text[(match2.Index + match2.Length)..]}";

                    _playList.Add(playInfo);

                    SymbolText.s_nodebases.Clear();
                    _symbolText.Parser.parser(_symbolText, text.Substring(playInfo.index, playInfo.length), cfg, SymbolText.s_nodebases, null);

                    foreach (var t in SymbolText.s_nodebases)
                    {
                        _nodeList.Enqueue(t);
                    }

                    SymbolText.s_nodebases.Clear();
                }
                else
                {
                    break;
                }
            }

            _text = text;

            if (_playList.Count > 0)
            {
                _isRunning = true;
                _isSkip = false;
                _coroutine = StartCoroutine(Run());
            }
            else
            {
                Complete();
            }

            return true;
        }

        private IEnumerator Run()
        {
            for (int i = 0; i < _playList.Count; i++)
            {
                var playInfo = _playList[i];

                var str = _text.Substring(playInfo.index, playInfo.length);
                var str1 = _text[..playInfo.index];
                var l_index = 0;
                var l_length = 0;
                var l_lastIndex = 0;

                while (true)
                {
                    var content = string.Empty;
                    var regex = string.Empty;

                    while (true)
                    {
                        var node = _nodeList.Peek();

                        if (node is HyperlinkNode or CartoonNode)
                        {
                            Match match1;
                            Match match2;
                            Match temp = null;

                            HyperlinkNode hyperlinkNode = null;

                            if (node is HyperlinkNode hyperlinkNode1)
                            {
                                hyperlinkNode = hyperlinkNode1;
                                match1 = Regex.Match(str, $"<hy t={hyperlinkNode.d_text} l={hyperlinkNode.d_link}[^<>]*>");
                                match2 = Regex.Match(str, $"#h{hyperlinkNode.d_text}\\[{hyperlinkNode.d_link}\\]#h");
                            }
                            else
                            {
                                var cartoonNode = (CartoonNode) node;
                                match1 = Regex.Match(str, $"<face n={cartoonNode.cartoon.name}[^<>]*>");
                                match2 = Regex.Match(str, $"#{cartoonNode.cartoon.name}");
                            }

                            if (match1.Success && match2.Success)
                            {
                                temp = match1.Index < match2.Index ? match1 : match2;
                            }
                            else if (match1.Success)
                            {
                                temp = match1;
                            }
                            else if (match2.Success)
                            {
                                temp = match2;
                            }

                            if (temp != null)
                            {
                                l_index = temp.Index;
                                l_length = temp.Length;
                                _nodeList.Dequeue();

                                if (hyperlinkNode == null)
                                {
                                    content = temp.Value;
                                }
                                else
                                {
                                    content = hyperlinkNode.d_text;
                                    regex = Regex.Replace(temp.Value, hyperlinkNode.d_text, "{0}");
                                }
                            }

                            break;
                        }

                        if (node is TextNode textNode)
                        {
                            var match = Regex.Match(str, textNode.d_text);

                            if (match.Success)
                            {
                                l_index = match.Index;
                                l_length = match.Length;
                                _nodeList.Dequeue();
                            }

                            break;
                        }

                        if (node is SpriteNode spriteNode)
                        {
                            var match = Regex.Match(str, $"<sprite n={spriteNode.sprite.name}[^<>]*>");

                            if (match.Success)
                            {
                                content = match.Value;
                                l_index = match.Index;
                                l_length = match.Length;
                                _nodeList.Dequeue();
                            }

                            break;
                        }

                        _nodeList.Dequeue();
                    }

                    _symbolText.text = _text[..(playInfo.index + l_lastIndex)];

                    if (string.IsNullOrEmpty(content) && string.IsNullOrEmpty(regex))
                    {
                        for (int j = 1; j <= l_length; j++)
                        {
                            yield return new WaitForSeconds(playInfo.speed);
                            _symbolText.text = $"{str1}{str[..(l_index + j)]}";
                        }
                    }
                    else if (string.IsNullOrEmpty(regex))
                    {
                        yield return new WaitForSeconds(playInfo.speed);
                        _symbolText.text = $"{str1}{str[..(l_index + l_length)]}";
                    }
                    else
                    {
                        for (int j = 1; j <= content.Length; j++)
                        {
                            yield return new WaitForSeconds(playInfo.speed);
                            _symbolText.text = $"{str1}{str[..l_index]}{string.Format(regex, content[..j])}";
                        }
                    }

                    l_lastIndex += l_index + l_length;

                    if (l_lastIndex >= playInfo.length)
                    {
                        break;
                    }

                    str = str[(l_index + l_length)..];
                    str1 = _text[..(playInfo.index + l_lastIndex)];
                }
            }

            Complete();
        }

        private void Complete()
        {
            Stop();
            _symbolText.text = _text;
            _onComplete?.Invoke();
            _onComplete = null;
        }

        private void Stop()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _isRunning = false;
            _isSkip = false;
        }

        public void Skip()
        {
            if (_isRunning && _isSkip)
            {
                Complete();
            }
        }
    }
}