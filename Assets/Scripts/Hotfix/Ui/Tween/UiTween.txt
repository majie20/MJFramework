using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MGame.Hotfix
{
    public static class UiTween
    {
        public static float SCREEN_WIDTH = 1080;
        public static float SCREEN_HEIGHT = 1920;

        private static readonly List<System.Action<object>> calls = new List<Action<object>>();
        private static readonly ArrayList values = new ArrayList();

        public static void UiOpenTween(this MonoBehaviour component, RectTransform tran, System.Action backCall = null)
        {
            var enumerator = component.GetType().GetCustomAttributes<BaseTweenAttribute>();
            DisposeTween(enumerator, tran, UseWay.Open, backCall);
        }

        public static void UiOpenTween(BaseTweenAttribute[] enumerator, RectTransform tran, System.Action backCall = null)
        {
            DisposeTween(enumerator, tran, UseWay.Open, backCall);
        }

        public static void UiCloseTween(this MonoBehaviour component, RectTransform tran, System.Action backCall = null)
        {
            var enumerator = component.GetType().GetCustomAttributes<BaseTweenAttribute>();
            DisposeTween(enumerator, tran, UseWay.Close, backCall);
        }

        public static void UiCloseTween(BaseTweenAttribute[] enumerator, RectTransform tran, System.Action backCall = null)
        {
            DisposeTween(enumerator, tran, UseWay.Close, backCall);
        }

        private static void DisposeTween(IEnumerable<BaseTweenAttribute> enumerator, RectTransform tran, UseWay way, System.Action backCall)
        {
            if (enumerator == null)
            {
                backCall?.Invoke();
                return;
            }
            var sequence = DOTween.Sequence();

            foreach (var attribute in enumerator)
            {
                if (attribute.useWay == way)
                {
                    if (attribute is TranslateTweenAttribute translate)
                    {
                        var value = 100;
                        Tweener tween = null;
                        switch (translate.direction)
                        {
                            case Direction.Left:
                                tween = tran.DOAnchorPos(
                                    new Vector2(-(SCREEN_WIDTH + tran.rect.width) / 2 - value, tran.anchoredPosition.y),
                                    translate.duration);
                                break;

                            case Direction.Right:
                                tween = tran.DOAnchorPos(
                                    new Vector2((SCREEN_WIDTH + tran.rect.width) / 2 + value, tran.anchoredPosition.y),
                                    translate.duration);
                                break;

                            case Direction.Up:
                                tween = tran.DOAnchorPos(
                                    new Vector2(tran.anchoredPosition.x,
                                        (SCREEN_HEIGHT + tran.rect.height) / 2 + value), translate.duration);
                                break;

                            case Direction.Down:
                                tween = tran.DOAnchorPos(
                                    new Vector2(tran.anchoredPosition.x,
                                        -(SCREEN_HEIGHT + tran.rect.height) / 2 - value), translate.duration);
                                break;

                            case Direction.LeftAndUp:
                                tween = tran.DOAnchorPos(
                                    new Vector2(-(SCREEN_WIDTH + tran.rect.width) / 2 - value,
                                        (SCREEN_HEIGHT + tran.rect.height) / 2 + value), translate.duration);
                                break;

                            case Direction.RightAndUp:
                                tween = tran.DOAnchorPos(
                                    new Vector2((SCREEN_WIDTH + tran.rect.width) / 2 + value,
                                        (SCREEN_HEIGHT + tran.rect.height) / 2 + value), translate.duration);
                                break;

                            case Direction.LeftAndDown:
                                tween = tran.DOAnchorPos(
                                    new Vector2(-(SCREEN_WIDTH + tran.rect.width) / 2 - value,
                                        -(SCREEN_HEIGHT + tran.rect.height) / 2 - value), translate.duration);
                                break;

                            case Direction.RightAndDown:
                                tween = tran.DOAnchorPos(
                                    new Vector2((SCREEN_WIDTH + tran.rect.width) / 2 + value,
                                        -(SCREEN_HEIGHT + tran.rect.height) / 2 - value), translate.duration);
                                break;
                        }

                        if (way == UseWay.Open)
                        {
                            tween = tween.From();
                        }
                        else if (way == UseWay.Close)
                        {
                            calls.Add((obj) => { tran.anchoredPosition = (Vector2)obj; });
                            values.Add(tran.anchoredPosition);
                        }

                        sequence.Insert(0, tween.SetEase(translate.ease));
                    }
                    else if (attribute is ScaleTweenAttribute scale)
                    {
                        var tween = tran.DOScale(new Vector3(scale.x, scale.y, scale.z), scale.duration).SetEase(scale.ease);

                        if (way == UseWay.Open)
                        {
                            tween.From();
                        }
                        else if (way == UseWay.Close)
                        {
                            calls.Add((obj) => { tran.localScale = (Vector3)obj; });
                            values.Add(tran.localScale);
                        }

                        sequence.Insert(0, tween);
                    }
                    else if (attribute is RotateTweenAttribute rotate)
                    {
                        var tween = tran.DORotate(new Vector3(rotate.x, rotate.y, rotate.z), rotate.duration,
                            RotateMode.FastBeyond360).SetEase(rotate.ease);

                        if (way == UseWay.Open)
                        {
                            tween.From();
                        }
                        else if (way == UseWay.Close)
                        {
                            calls.Add((obj) => { tran.eulerAngles = (Vector3)obj; });
                            values.Add(tran.eulerAngles);
                        }

                        sequence.Insert(0, tween);
                    }
                    else if (attribute is AlphaTweenAttribute alpha)
                    {
                        var canvasGroup = tran.GetComponent<CanvasGroup>();
                        if (canvasGroup != null)
                        {
                            var tween = canvasGroup.DOFade(alpha.alpha, alpha.duration).SetEase(alpha.ease);

                            if (way == UseWay.Open)
                            {
                                tween.From();
                            }
                            else if (way == UseWay.Close)
                            {
                                calls.Add((obj) => { canvasGroup.alpha = (float)obj; });
                                values.Add(canvasGroup.alpha);
                            }

                            sequence.Insert(0, tween);
                        }
                        else
                        {
                            var images = tran.GetComponentsInChildren<Image>();
                            for (int i = 0; i < images.Length; i++)
                            {
                                var tween = images[i].DOFade(alpha.alpha, alpha.duration).SetEase(alpha.ease);

                                if (way == UseWay.Open)
                                {
                                    tween.From();
                                }
                                else if (way == UseWay.Close)
                                {
                                    var image = images[i];
                                    calls.Add((obj) =>
                                    {
                                        var color = image.color;
                                        color.a = (float)obj;
                                        image.color = color;
                                    });
                                    values.Add(image.color.a);
                                }

                                sequence.Insert(0, tween);
                            }

                            var texts = tran.GetComponentsInChildren<Text>();
                            for (int i = 0; i < texts.Length; i++)
                            {
                                var tween = texts[i].DOFade(alpha.alpha, alpha.duration).SetEase(alpha.ease);

                                if (way == UseWay.Open)
                                {
                                    tween.From();
                                }
                                else if (way == UseWay.Close)
                                {
                                    var text = texts[i];
                                    calls.Add((obj) =>
                                    {
                                        var color = text.color;
                                        color.a = (float)obj;
                                        text.color = color;
                                    });
                                    values.Add(text.color.a);
                                }

                                sequence.Insert(0, tween);
                            }
                        }
                    }
                }
            }

            if (calls != null)
            {
                sequence.AppendCallback((() =>
                {
                    for (int i = 0; i < calls.Count; i++)
                    {
                        calls[i](values[i]);
                    }
                    calls.Clear();
                    values.Clear();
                }));
            }
            if (backCall != null)
                sequence.AppendCallback(new TweenCallback(backCall));
        }
    }
}