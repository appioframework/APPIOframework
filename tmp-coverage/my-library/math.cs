using System;

namespace MyLibrary
{
    static public class Math
    {
        static public float Add( float a, float b )
        {
            return a + b;
        }

        static public bool IsValidPercent( int percent )
        {
            if ( percent <    0 ) { return false; }
            if ( percent >= 100 ) { return false; }
            
            return true;
        }
    }
}
