using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using ITF.DataServices.SDK;
using ITF.MediaPlatform.API.App_Start;
using Newtonsoft.Json;
using Ninject;
using NLog;
using Xunit;
using Xunit.Sdk;

namespace ITF.MediaPlatform.API.Tests
{
    public class IisExpressFixture : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public const int Port = 58851;
        public const string ApplicationName = "ITF.MediaPlatform.API";

        private static Process _iisProcess;
        private static readonly Thread IisExpressThread = new Thread(StartIisExpress) { IsBackground = true };
        private static readonly List<ManualResetEvent> IisExpressTestHandles = new List<ManualResetEvent>();
        private static readonly Uri ServiceBaseUrl = new Uri($"http://localhost:{Port}/");

        public IKernel Kernel { get; private set; }

        public IisExpressFixture()
        {
            Logger.Info("Fixture initializing...");
            Kernel = NinjectWebCommon.CreateKernel();
            IisExpressThread.Start();
        }

        private static void StartIisExpress()
        {
            Logger.Info("IIS Express starting...");

            Process.GetProcessesByName("iisexpress").ToList().ForEach(x =>
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug($"Killing process {x.Id} ...");
                }
                x.Kill();
            });

            var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            solutionFolder = solutionFolder ?? string.Empty;
            var appLocation = Path.Combine(solutionFolder, ApplicationName);
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = $"/path:\"{appLocation}\" /port:{Port} /systray:false"
            };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                                ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            try
            {
                _iisProcess = new Process { StartInfo = startInfo };

                _iisProcess.Start();
                _iisProcess.WaitForExit();
            }
            catch(Exception e)
            {
                Logger.Error(e, "Error when starting IIS Express");
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                if (IisExpressTestHandles != null && IisExpressTestHandles.Any())
                {
                    Logger.Info("Waiting for all test finished...");
                    var arrHandles = IisExpressTestHandles.ToArray<WaitHandle>();
                    //WaitHandle.WaitAll(arrHandles);
                    var splitArrHandles = arrHandles.Split(64);
                    splitArrHandles.ToList().ForEach(x =>
                    {
                        var waitHandles = x as WaitHandle[] ?? x.ToArray();
                        Logger.Info($"Wait for handles: {waitHandles.Length}");
                        WaitHandle.WaitAll(waitHandles);
                    });
                    Thread.Sleep(100);
                }

                Logger.Info("Fixture disposing...");
                if (!_iisProcess.HasExited)
                {
                    Logger.Info("IIS Express closing...");
                    _iisProcess.CloseMainWindow();
                    //_iisProcess.Dispose();
                    _iisProcess.Kill();
                }

                if (IisExpressThread != null && IisExpressThread.IsAlive)
                {
                    IisExpressThread.Join();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Unexpected error while disposing IIS Express");
                throw;
            }
        }

        #region Calling webapi hosted in IIS Express

        public static void CallWebApiInThreadPool(object controllerResult, params string[] args)
        {
            var handle = new ManualResetEvent(false);
            IisExpressTestHandles.Add(handle);
            var callWebApiWrapper = new Action<object, string[]>(delegate
            {
                try
                {
                    CallWebApi(controllerResult, args);
                }
                catch (XunitException e)
                {
                    var requestUri = string.Join("/", args.Where(x => !string.IsNullOrEmpty(x)));
                    Logger.Error(e, $"Test failed: {requestUri}");
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Unexpected error while calling wepapi");
                }
                finally
                {
                    handle.Set();
                }
            });
            ThreadPool.QueueUserWorkItem(x => callWebApiWrapper(controllerResult, args));
        }

        public static void CallWebApi(object controllerResult, params string[] args)
        {
            var client = new HttpClient
            {
                BaseAddress = ServiceBaseUrl
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestUri = string.Join("/", args.Where(x => !string.IsNullOrEmpty(x)));
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"Calling WebApi at RequestUrl: {requestUri}");
            }
            var response = client.GetAsync(requestUri).Result;
            Assert.True(response.IsSuccessStatusCode);
            var webApiResult = response.Content.ReadAsAsync(controllerResult.GetType()).Result;
            Assert.Equal(JsonConvert.SerializeObject(webApiResult), JsonConvert.SerializeObject(controllerResult));
        }

        #endregion
    }

    [CollectionDefinition("IisExpressTest")]
    public class IisExpressCollection : ICollectionFixture<IisExpressFixture>
    {
    }
}
