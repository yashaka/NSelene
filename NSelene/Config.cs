using System;
namespace NSelene
{

    public static class Config
    {
        public static double Timeout = 4;
        public static double PollDuringWaits = 0.1;
        public static DriverStorage DriverStorage = new ThreadLocalDriverStorage();
    }
}

