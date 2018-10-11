using Microsoft.VisualStudio.TestTools.UnitTesting;

using MyLibrary;

namespace TestMyLibrary
{
    [TestClass]
    public class TestMath
    {
        // [TestMethod]
        // public void AddShouldReturnSumOfAPlusB()
        // {
        //     const float a           = 1.0f;
        //     const float b           = 2.0f;
        //     const float expectedSum = 3.0f;

        //     var sum
        //         = Math.Add( a, b );

        //     Assert.AreEqual( expectedSum, sum );
        // }

        [DataRow( -1 )]
        [DataRow( -2354 )]
        [DataTestMethod]
        public void IsValidPercentShouldReturnFalseForNegativeValues( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsFalse( result );
        }

        [DataRow( 150 )]
        [DataRow( 472935 )]
        [DataTestMethod]
        public void IsValidPercentShouldReturnFalseForGreaterThan100( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsFalse( result );
        }

        [DataRow( 60 )]
        [DataRow( 29 )]
        [DataRow( 1 )]
        [DataRow( 99 )]
        [DataRow( 0 )]
        [DataTestMethod]
        public void IsValidPercentShouldReturnTrueFor0To100( int percent )
        {
            var result
                = Math.IsValidPercent( percent );

            Assert.IsTrue( result );
        }
    }
}
