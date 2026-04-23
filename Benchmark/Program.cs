using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    // Simplified versions of the models for benchmarking
    public class TDOTCamera
    {
        public int Id { get; set; }
        public string Jurisdiction { get; set; }
        public string Active { get; set; }
        public string Route { get; set; }
        public string Title { get; set; }
        public string MileMarker { get; set; }
    }

    public class Camera
    {
        public int Id { get; set; }
        public string Road { get; set; }
        public float? MM { get; set; }

        public Camera(TDOTCamera camera)
        {
            Id = camera.Id;
            Road = camera.Route;
            MM = string.IsNullOrEmpty(camera.MileMarker) ? null : float.Parse(camera.MileMarker);
        }
    }

    [MemoryDiagnoser]
    public class CameraBenchmark
    {
        private List<TDOTCamera> _tdotCameras;
        private bool _isDebugEnabled = false; // Simulated logger state

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _tdotCameras = new List<TDOTCamera>();
            for (int i = 0; i < 1000; i++)
            {
                string[] jurisdictions = { "Knoxville", "Nashville", "Memphis", "Chattanooga" };
                string[] statuses = { "true", "false" };
                string[] routes = { "I-40", "I-75", "I-81", "I-26", "SR-162" };

                _tdotCameras.Add(new TDOTCamera
                {
                    Id = i,
                    Jurisdiction = jurisdictions[random.Next(jurisdictions.Length)],
                    Active = statuses[random.Next(statuses.Length)],
                    Route = routes[random.Next(routes.Length)],
                    MileMarker = random.Next(0, 400).ToString()
                });
            }
        }

        [Benchmark(Baseline = true)]
        public List<Camera> Original()
        {
            var tdotCameras = _tdotCameras.Where(x => x.Jurisdiction == "Knoxville" && x.Active == "true").OrderBy(x => x.Id).ToList();
            if (_isDebugEnabled)
            {
                var routes = string.Join(", ", tdotCameras.Select(x => x.Route).Distinct());
            }
            var cameras = tdotCameras.Select(x => new Camera(x)).Where(x => x.Road != "I-26" && x.Road != "I-81").OrderBy(x => x.Road).ThenBy(x => x.MM ?? float.MaxValue).ToList();
            return cameras;
        }

        [Benchmark]
        public List<Camera> FullyDeferredNoDebug()
        {
            var tdotCamerasQuery = _tdotCameras.Where(x => x.Jurisdiction == "Knoxville" && x.Active == "true");

            if (_isDebugEnabled)
            {
                var routes = string.Join(", ", tdotCamerasQuery.Select(x => x.Route).Distinct());
            }

            var cameras = tdotCamerasQuery.Select(x => new Camera(x)).Where(x => x.Road != "I-26" && x.Road != "I-81").OrderBy(x => x.Road).ThenBy(x => x.MM ?? float.MaxValue).ToList();
            return cameras;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CameraBenchmark>();
        }
    }
}
