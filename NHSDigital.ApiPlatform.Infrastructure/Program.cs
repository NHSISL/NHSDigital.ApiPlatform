// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using NHSDigital.ApiPlatform.Infrastructure.Services;

namespace NHSDigital.ApiPlatform.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "NHSDigital.ApiPlatform.Sdk",
                dotNetVersion: "10.0.100");

            scriptGenerationService.GeneratePrLintScript(branchName: "main");
        }
    }
}
