using NUnit.Framework;

using MyLibrary;

namespace TestMyLibrary
{
    public class TestMath
    {
        // [Test]
        // public void AddShouldReturnSumOfAPlusB()
        // {
        //     const float a           = 1.0f;
        //     const float b           = 2.0f;
        //     const float expectedSum = 3.0f;

        //     var sum
        //         = Math.Add( a, b );

        //     Assert.AreEqual( expectedSum, sum );
        // }

        [TestCase( -   1 )]
        [TestCase( -2354 )]
        public void IsValidPercentShouldReturnFalseForNegativeValues( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsFalse( result );
        }

        [TestCase(    150 )]
        [TestCase( 472935 )]
        public void IsValidPercentShouldReturnFalseForGreaterThan100( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsFalse( result );
        }

        [TestCase( 60 )]
        [TestCase( 29 )]
        [TestCase(  1 )]
        [TestCase( 99 )]
        [TestCase(  0 )]
        public void IsValidPercentShouldReturnTrueFor0To100( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsTrue( result );
        }
    }
}
