﻿//-----------------------------------------------------------------------
// <copyright file="RestartTests.cs" company="Outercurve Foundation">
//   Copyright (c) 2004, Outercurve Foundation.
//   This software is released under Microsoft Reciprocal License (MS-RL).
//   The license and further copyright text can be found in the file
//   LICENSE.TXT at the root directory of the distribution.
// </copyright>
//-----------------------------------------------------------------------

namespace WixTest.BurnIntegrationTests
{
    using System.Collections.Generic;
    using System.IO;
    using Xunit;

    /// <summary>
    /// Restart Burn tests.
    /// </summary>
    public class RestartTests : BurnTestBase
    {
        private PackageBuilder packageA;
        private BundleBuilder bundleA;

        [NamedFact(Skip="Disabling this test since it does not consistently lock the file in a way that the Windows Installer sees FilesInUse.")]
        [Trait("RuntimeTest", "true")]
        public void Burn_RetryThenCancel()
        {
            this.Initialize(dataFolder: "BasicTests");

            this.SetPackageRetryExecuteFilesInUse("PackageA", 1);

            string bundleAPath = this.GetBundleA().Output;
            BundleInstaller installA = new BundleInstaller(this, bundleAPath);

            // Lock the file that will be installed.
            string targetInstallFile = this.GetTestInstallFolder("A\\A.wxs");
            Directory.CreateDirectory(Path.GetDirectoryName(targetInstallFile));
            using (FileStream lockTargetFile = new FileStream(targetInstallFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                installA.Install(expectedExitCode:(int)MSIExec.MSIExecReturnCode.ERROR_INSTALL_USEREXIT);
            }

            this.Completed();
        }

        private PackageBuilder GetPackageA()
        {
            if (null == this.packageA)
            {
                this.packageA = this.CreatePackage("A");
            }

            return this.packageA;
        }

        private BundleBuilder GetBundleA(Dictionary<string, string> bindPaths = null)
        {
            if (null == bindPaths)
            {
                string packageAPath = this.GetPackageA().Output;
                bindPaths = new Dictionary<string, string>() { { "packageA", packageAPath } };
            }

            if (null == this.bundleA)
            {
                this.bundleA = this.CreateBundle("BundleA", bindPaths);
            }

            return this.bundleA;
        }
    }
}
