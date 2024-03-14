using System;
using System.Collections.Generic;
using System.Linq;
using DC.ServiceLocator;
using UnityEngine;

namespace DC.MessageService
{
    public class ExtendedBehaviour : MonoBehaviour
    {
        protected ITinyMessengerHub EventHub => _eventHub ??= BaseLocator.Find<ITinyMessengerHub>();

        private Dictionary<Type, TinyMessageSubscriptionToken> _tokens;
        private ITinyMessengerHub _eventHub;

        protected virtual void Awake()
        {
            _tokens = new Dictionary<Type, TinyMessageSubscriptionToken>();
        }
    
        protected void Subscribe<T>(Action<T> action) where T : class, ITinyMessage
        {
            if (!BaseLocator.DoesServiceExist(typeof(ITinyMessengerHub)))
            {
                BaseLocator.AddNewService<ITinyMessengerHub>(new TinyMessengerHub());
            }
            
            _tokens.Add(typeof(T), EventHub.Subscribe(action));
        }
        
        protected void Unsubscribe<T>()
        {
            var type = typeof(T);
            
            foreach (var token in
                     _tokens.Where(token => token.Key == type))
            {
                Debug.Log($"{gameObject.name} has unsubscribed from message type {token.Key.Name}");
                EventHub.Unsubscribe(token.Value);
                _tokens.Remove(token.Key);

                break;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_tokens.Count <= 0) return;
            
            Debug.Log($"{gameObject.name} is cleaning up it's messages.");
            
            foreach (var token in _tokens)
            {
                EventHub.Unsubscribe(token.Value);
            }
            _tokens.Clear();
        }
    }
}