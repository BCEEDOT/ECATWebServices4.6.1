using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Breeze.ContextProvider.EF6;
using Ecat.Data.Contexts;
using Ecat.Data.Models.Canvas;
using Ecat.Data.Models.Common;

namespace Ecat.Business.Utilities
{
    public static class CanvasOps
    {
        //TODO: Update once we have production Canvas
        private static readonly string canvasApiUrl = "https://lms.stag.af.edu/api/v1/";


        public static async Task<HttpResponseMessage> GetResponse(int facultyId, string apiResource, CanvasLogin canvasLogin)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + canvasLogin.AccessToken);

            var apiAddr = new Uri(canvasApiUrl + apiResource);

            return await client.GetAsync(apiAddr);

        }

        //public static List<HttpResponseMessage> ProcessRequestsInParallel(List<string> apiResourceCalls, int facultyId, ReconcileResult reconResult, EFContextProvider<EcatContext> ctxManager)
        //{
        //    var exceptions = new ConcurrentQueue<Exception>();
        //    var responses = new ConcurrentQueue<HttpResponseMessage>();

        //    Parallel.ForEach(apiResourceCalls, async apiResourceCall =>
        //    {
        //        try
        //        {
        //            var response = await GetResponse(facultyId, reconResult, apiResourceCall, ctxManager);
        //            responses.Enqueue(response);
        //        }

        //        catch (Exception e)
        //        {
        //            exceptions.Enqueue(e);
        //        }
        //    });

        //    if (exceptions.Count > 0) throw new AggregateException(exceptions);

        //    return responses;
        //}
    }
}
