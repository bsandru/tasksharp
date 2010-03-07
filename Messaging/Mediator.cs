//This file is part of TaskSharp.
//
//TaskSharp is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//TaskSharp is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with TaskSharp.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TaskSharp.Messaging
{
    public static class Mediator
    {
        private static readonly Dictionary<Type, List<Receiver>> _subscriptions = new Dictionary<Type, List<Receiver>>();

        public static void Send<T>() where T : MediatorMessage, new()
        {
            Send<T>(Activator.CreateInstance<T>());
        }
        public static void Send<T>(T message) where T : MediatorMessage
        {
            if (message == null || !_subscriptions.ContainsKey(typeof(T)))
                return;
            var remove = new List<Receiver>();
            foreach (var receiver in _subscriptions[typeof(T)])
            {
                if (receiver.IsAlive)
                    receiver.Send(message);
                else
                    remove.Add(receiver);
            }
            remove.ForEach(r => _subscriptions[typeof(T)].Remove(r));
        }

        public static void Subscribe(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "instance is null.");
            var instanceType = instance.GetType();
            Subscribe(instanceType);
            SubscribeInternal(instance, instanceType);
        }

        public static void Subscribe(Type staticType)
        {
            if (staticType == null)
                throw new ArgumentNullException("staticType", "staticType is null.");

            SubscribeInternal(null, staticType);
        }
        private static void SubscribeInternal(object instance, Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | ((instance == null) ? BindingFlags.Static : BindingFlags.Instance)))
            {
                //subscribe all methods that take a MediatorMessage as single parameter
                var parameters = method.GetParameters();
                var firstParameter = parameters.FirstOrDefault();
                if (parameters.Length == 1 && typeof(MediatorMessage).IsAssignableFrom(firstParameter.ParameterType))
                    AddSubscriber(instance, method, firstParameter.ParameterType);
            }
        }
        private static void AddSubscriber(object instance, MethodInfo method, Type parameterType)
        {
            if (!_subscriptions.ContainsKey(parameterType))
                _subscriptions.Add(parameterType, new List<Receiver>());
            WeakReference reference = null;
            if (instance != null)
                reference = new WeakReference(instance);
            _subscriptions[parameterType].Add(new Receiver { Instance = reference, Method = method });
        }

        private class Receiver
        {
            public WeakReference Instance { get; set; }
            public MethodInfo Method { get; set; }
            public bool IsAlive
            {
                get { return Method != null && (Instance == null || Instance.IsAlive); }
            }
            public void Send(MediatorMessage message)
            {
                Method.Invoke(Instance != null ? Instance.Target : null, new[] { message });
            }
        }
    }
}
