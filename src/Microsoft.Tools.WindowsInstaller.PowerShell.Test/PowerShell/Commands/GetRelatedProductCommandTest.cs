﻿// Copyright (C) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Tools.WindowsInstaller.PowerShell.Commands
{
    /// <summary>
    /// Unit and functional tests for <see cref="GetRelatedProductCommand"/>.
    ///</summary>
    [TestClass]
    public class GetRelatedProductCommandTest : CommandTestBase
    {
        /// <summary>
        /// Enumerates related products.
        /// </summary>
        [TestMethod]
        [Description("Enumerates related products")]
        public void EnumerateRelatedProducts()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msirelatedproductinfo -upgradecode ""{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}"""))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    // Import our registry entries.
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual<int>(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value as string);
                }
            }
        }

        [TestMethod]
        [Description("Tests chained execution of get-msirelatedproductinfo")]
        [WorkItem(9464)]
        public void GetRelatedProductChainedExecution()
        {
            using (Pipeline p = TestRunspace.CreatePipeline(@"get-msirelatedproductinfo '{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}' | add-member -name UpgradeCode -type noteproperty -value '{C1482EA4-07D3-4261-9741-7CEDE6A8C25A}' -passthru | get-msirelatedproductinfo"))
            {
                using (MockRegistry reg = new MockRegistry())
                {
                    reg.Import(@"registry.xml");

                    Collection<PSObject> objs = p.Invoke();

                    Assert.AreEqual(1, objs.Count);
                    Assert.AreEqual<string>("{89F4137D-6C26-4A84-BDB8-2E5A4BB71E00}", objs[0].Properties["ProductCode"].Value as string);
                }
            }
        }
    }
}
