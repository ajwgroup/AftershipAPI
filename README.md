# AftershipAPI

[![License - MIT](https://img.shields.io/github/license/ajwgroup/AftershipAPI.svg?style=flat-square)](https://github.com/ajwgroup/AftershipAPI/blob/master/LICENSE)
[![Build Status](https://img.shields.io/travis/com/ajwgroup/AftershipAPI/master.svg?logo=travis&style=flat-square)](https://travis-ci.com/ajwgroup/AftershipAPI)
[![Issues](https://img.shields.io/github/issues/ajwgroup/AftershipAPI.svg?style=flat-square)](https://github.com/ajwgroup/AftershipAPI/issues)
![Latest Nuget Release](https://img.shields.io/nuget/v/AftershipAPI.svg?style=flat-square&link=http://www.nuget.org/packages/AftershipAPI/&link=http://www.nuget.org/packages/AftershipAPI/)
[![Latest Nuget Pre-Release](https://img.shields.io/nuget/vpre/AftershipAPI.svg?style=flat-square&colorB=yellow&label=nuget-prerelease)](https://www.nuget.org/packages/AftershipAPI/)
[![Coverage Status](https://img.shields.io/codecov/c/github/ajwgroup/AftershipAPI.svg?logo=codecov&style=flat-square)](https://codecov.io/gh/ajwgroup/AftershipAPI)
[![Last commit](https://img.shields.io/github/last-commit/ajwgroup/AftershipAPI.svg?style=flat-square)](https://github.com/ajwgroup/AftershipAPI)

This is an unofficial Aftership SDK for .NET Standard version 2.1.

[Aftership](https://www.aftership.com) provides a parcel tracking service that works with hundreds of couriers from arround the world.

## Simple Examples

``` csharp
using AftershipAPI;
using System;

namespace AftershipAPIExample
{
    class Program
    {
        static int Main(string[] args)
        {
            // Create an instance of ConnectionAPI using the token
            ConnectionAPI connection = new ConnectionAPI("AftershipApiKey");

            // Create a new tracking to search for
            Tracking trackingToFind = new Tracking("trackingnumber"){ Slug = "slug" };

            // Must provide tracking number and slug to search for tracking
            Tracking tracking = GetTrackingByNumber(connection, trackingToFind);

            if(tracking != null)
            {
                Checkpoint checkpoint = GetLastCheckpoint(connection, tracking);
            }
        }

        public Tracking GetTrackingByNumber(ConnectionAPI connection, Tracking trackingToFind)
        {
            try
            {
                var tracking = connection.GetTrackingByNumber(trackingToFind);

                Console.WriteLine($"Tracking active?: {tracking.Active}");

                return tracking;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There's be an error: {ex.Message}");
                return null;
            }
        }

        public Checkpoint GetLastCheckpoint(ConnectionAPI connection, Tracking tracking)
        {
            var lastCheckpoint = connection.GetLastCheckpoint(tracking);

            Console.WriteLine($"Last checked at: {lastCheckpoint.CheckpointTime}");

            return lastCheckpoint;
        }
    }
}
```

## Tips

- Use exception handling when using the ConnectionAPI, this calls the Aftership service and can produce errors.
