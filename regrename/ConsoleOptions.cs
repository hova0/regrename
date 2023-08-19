using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regrename
{
    public class ConsoleOptions : List<ConsoleOption>
    {
        private enum ArgumentState
        {
            Uninitialized = 0,
            Key = 1,
            Value = 2,
            KeyAndValue = 3 // should never use this
        }
        //public ConsoleOptionList OptionList = new ConsoleOptionList();

        public ConsoleOptions(string[] args)
        {
            ArgumentState _state = ArgumentState.Uninitialized;
            ConsoleOption co = new ConsoleOption();
            int _ordinal = 0;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                // Keyed arguments
                int keyprefixlength = GetKeyPrefixLength(arg);
                if (keyprefixlength > 0)
                {
                    _state = ArgumentState.Key;
                    co.key = arg.Substring(keyprefixlength);

                    int potentialvalueindex = co.key.IndexOfAny(new char[] { '=', ':' });
                    if (potentialvalueindex != -1)
                    {
                        // "--key=value" argument
                        co.value = co.key.Substring(potentialvalueindex + 1);
                        co.key = co.key.Substring(0, potentialvalueindex);
                        this.Add(co);
                        _state = ArgumentState.Uninitialized;
                        continue;
                    }
                    // no value in arg, "--key ".  value will be in next argument hopefully
                }
                else
                {
                    // value was in the next argument.  previous argument was key as in "--key value"
                    if (_state == ArgumentState.Key)
                    {
                        co.value = arg;
                        this.Add(co);
                        co = new ConsoleOption();
                        _state = ArgumentState.Uninitialized;
                        continue;

                    }
                    // ordinal argument.    that is a value without a key.
                    else if (_state == ArgumentState.Uninitialized)
                    {
                        co.ordinal = _ordinal;
                        _ordinal++;
                        co.value = arg;
                        this.Add(co);
                        co = new ConsoleOption();
                        _state = ArgumentState.Uninitialized;
                    }
                    else
                    {
                        throw new Exception("Bad State : " + _state.ToString() + " with arg " + arg);
                    }
                }
            }
            // last argument (with no value)
            if (_state == ArgumentState.Key)
                this.Add(co);
        }

        public ConsoleOption? GetByKey(string key)
        {
            for (int i = 0; i < this.Count; i++)
            {
                ConsoleOption o = this[i];
                if (!string.IsNullOrEmpty(o.key) && o.key == key)
                    return o;
            }
            return null;
        }

        public ConsoleOption? GetByOrdinal(int ordinal)
        {
            for (int i = 0; i < this.Count; i++)
            {
                ConsoleOption o = this[i];
                if (o.ordinal == ordinal)
                    return o;
            }
            return null;
        }
        private static int GetKeyPrefixLength(string arg)
        {
            if (arg.StartsWith("--") && arg.Length > 2)
                return 2;
            if (arg.StartsWith("-") && arg.Length > 1)
                return 1;
            if (arg.StartsWith("/") && arg.Length > 1)
                return 1;

            return 0;
        }

    }

    //public class ConsoleOptionList : List<ConsoleOption>
    //{
    //    private List<ConsoleOption> _list = new List<ConsoleOption>();

    //    public ConsoleOptionList()
    //    {

    //    }
    //    public ConsoleOptionList(List<ConsoleOption> options)
    //    {
    //        _list.AddRange(options.ToArray());
    //    }

   
    //    public void UpdateByOrdinal(int ordinal, ConsoleOption newvalue)
    //    {
    //        for (int i = 0; i < _list.Count; i++)
    //        {
    //            ConsoleOption o = _list[i];
    //            if (o.ordinal == ordinal)
    //            {
    //                _list[i] = newvalue;
    //                return;
    //            }
    //        }
    //    }
    //    public void UpdateByKey(string key, ConsoleOption newvalue)
    //    {
    //        for (int i = 0; i < _list.Count; i++)
    //        {
    //            ConsoleOption o = _list[i];
    //            if (!string.IsNullOrEmpty(o.key) && o.key == key)
    //            {
    //                _list[i] = newvalue;
    //                return;
    //            }
    //        }

    //    }

    //}

    public class ConsoleOption
    {
        public ConsoleOption() { this.value = ""; this.key = null; }
        public ConsoleOption(string _value) { this.value = _value; this.key = null; }
        public ConsoleOption(string _key, string _value) { this.key = _key; this.value = _value; }
        public ConsoleOption(string _value, int _ordinal) { this.value = _value; this.ordinal = _ordinal; this.key = null; }
        public string? key;
        public string? value;
        public int ordinal = -1;
    }
}
