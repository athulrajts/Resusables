﻿using KEI.Infrastructure.Server;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCPIPClient.Models
{
    public class InputParameter : BindableBase
    {
        private Type type = typeof(string);
        public Type Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        private string _value = string.Empty;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public static List<Type> AllowedTypes = new List<Type>
        {
            typeof(uint),
            typeof(float),
            typeof(double),
            typeof(string)
        };

        public byte[] GetBytes()
        {
            byte[] inputBytes = new byte[1];

            if (Type == typeof(uint))
            {
                if (uint.TryParse(Value, out uint value))
                {
                    inputBytes = BitConverter.GetBytes(value);
                }
            }
            else if (Type == typeof(float))
            {
                if (float.TryParse(Value, out float value))
                {
                    inputBytes = BitConverter.GetBytes(value);
                }
            }
            else if (Type == typeof(double))
            {
                if (double.TryParse(Value, out double value))
                {
                    inputBytes = BitConverter.GetBytes(value);
                }
            }
            else if (Type == typeof(string))
            {
                if(string.IsNullOrEmpty(Value) == false)
                {
                    var lengthBytes = BitConverter.GetBytes((uint)Value.Length);
                    inputBytes = BufferBuilder.Combine(lengthBytes, Encoding.ASCII.GetBytes(Value));
                }
            }

            return inputBytes;
          
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Value)) return false;

            if (Type == typeof(uint))
            {
                return uint.TryParse(Value?.ToString(), out uint value);
            }
            else if (Type == typeof(float))
            {
                return float.TryParse(Value?.ToString(), out float value);
            }
            else if (Type == typeof(double))
            {
                return double.TryParse(Value?.ToString(), out double value);
            }
            else if (Type == typeof(string))
            {
                return !string.IsNullOrEmpty(Value?.ToString());
            }

            return true;
        }
    }
}
