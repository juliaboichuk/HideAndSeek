using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeelTests
{
    public class MockRandomWithValueList : System.Random
    {
        private Queue<int> valuesToReturn;
        public MockRandomWithValueList(IEnumerable<int> values) =>
            valuesToReturn = new Queue<int>(values);
        public int NextValue()
        {
            var nextValue = valuesToReturn.Dequeue();
            valuesToReturn.Enqueue(nextValue);
            return nextValue;
        }
        public override int Next() => NextValue();
        public override int Next(int maxValue) => Next(0, maxValue);
        public override int Next(int minValue, int maxValue)
        {
            var next = NextValue();
            return next >= minValue && next < maxValue ? next : minValue;
        }
    }
}
