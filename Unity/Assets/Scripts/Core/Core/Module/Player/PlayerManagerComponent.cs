using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Model
{
    [LifeCycle]
    public class PlayerManagerComponent : Component, IAwake
    {
        public InputSystemUIInputModule InputSystemUIInputModule;

        private Dictionary<int, long> _players;

        public void Awake()
        {
            _players = new Dictionary<int, long>();
            InputSystemUIInputModule = GameObject.Find("Init/EventSystem").GetComponent<InputSystemUIInputModule>();

            //InputSystem.onDeviceChange += (device, change) =>
            //{
            //        NLog.Log.Error($"aaaaaaaa----{change}-----{device.name}----{device.displayName}");
            //        //switch (change)
            //        //{
            //        //    case InputDeviceChange.Added:
            //        //        // New Device.
            //        //        break;
            //        //    case InputDeviceChange.Disconnected:
            //        //        // Device got unplugged.
            //        //        break;
            //        //    case InputDeviceChange.Connected:
            //        //        // Plugged back in.
            //        //        break;
            //        //    case InputDeviceChange.Removed:
            //        //        // Remove from Input System entirely; by default, Devices stay in the system once discovered.
            //        //        break;
            //        //    default:
            //        //        // See InputDeviceChange reference for other event types.
            //        //        break;
            //        //}
            //};

            //InputSystem.onActionChange += (o, change) =>
            //{
            //    NLog.Log.Error($"bbbbbbb-----{o}-----{change}");
            //};
        }

        public override void Dispose()
        {
            InputSystemUIInputModule = null;
            _players = null;
            base.Dispose();
        }
    }
}