/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 
 *    Copyright 2019 (c) talsen team GmbH, http://talsen.team
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Appio.ObjectModel
{
    public class MessageLines : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> _messages = new List<KeyValuePair<string, string>>();

        public MessageLines()
        {
        }

        public MessageLines(MessageLines messages)
        {
            foreach (var msg in messages)
            {
                Add(msg.Key, msg.Value);
            }
        }

        public void Add(string messageHeader, string message)
        {
            _messages.Add(new KeyValuePair<string, string>(messageHeader, message));
        }

        public void Add(MessageLines lines)
        {
            foreach (var line in lines)
            {
                Add(line.Key, line.Value);
            }
        }

        public void Sort()
        {
            _messages.Sort(new KeyValuePairComparer());
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class KeyValuePairComparer : IComparer<KeyValuePair<string, string>>
        {
            public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return string.Compare(TrimStartDash(x.Key), TrimStartDash(y.Key), StringComparison.Ordinal);
            }

            private static string TrimStartDash(string value)
            {
                return value.TrimStart('-');
            }
        }
    }
}