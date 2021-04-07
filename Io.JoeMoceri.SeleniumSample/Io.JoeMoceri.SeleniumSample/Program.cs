using System;

namespace Io.JoeMoceri.SeleniumSample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var app = new App())
            {
                app.Run();
            }
        }
    }
}
