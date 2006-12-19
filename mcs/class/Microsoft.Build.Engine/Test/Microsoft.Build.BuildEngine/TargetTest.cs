//
// TargetTest.cs
//
// Author:
//   Marek Sieradzki (marek.sieradzki@gmail.com)
//
// (C) 2006 Marek Sieradzki
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace MonoTests.Microsoft.Build.BuildEngine {
	[TestFixture]
	public class TargetTest {
		
		Engine			engine;
		Project			project;
		
		[Test]
		public void TestFromXml1 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target'>
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			Assert.AreEqual (String.Empty, t [0].Condition, "A1");
			Assert.AreEqual (String.Empty, t [0].DependsOnTargets, "A2");
			Assert.IsFalse (t [0].IsImported, "A3");
			Assert.AreEqual ("Target", t [0].Name, "A4");
		}

		[Test]
		public void TestFromXml2 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' Condition='false' DependsOnTargets='X' >
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			Assert.AreEqual ("false", t [0].Condition, "A1");
			Assert.AreEqual ("X", t [0].DependsOnTargets, "A2");

			t [0].Condition = "true";
			t [0].DependsOnTargets = "A;B";

			Assert.AreEqual ("true", t [0].Condition, "A3");
			Assert.AreEqual ("A;B", t [0].DependsOnTargets, "A4");
		}

		[Test]
		public void TestAddNewTask1 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' >
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			BuildTask bt = t [0].AddNewTask ("Message");

			Assert.AreEqual ("Message", bt.Name, "A1");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestAddNewTask2 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' >
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			t [0].AddNewTask (null);
		}

		[Test]
		public void TestGetEnumerator ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' >
						<Message Text='text' />
						<Warning Text='text' />
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			IEnumerator e = t [0].GetEnumerator ();
			e.MoveNext ();
			Assert.AreEqual ("Message", ((BuildTask) e.Current).Name, "A1");
			e.MoveNext ();
			Assert.AreEqual ("Warning", ((BuildTask) e.Current).Name, "A2");
			Assert.IsFalse (e.MoveNext (), "A3");
		}

		[Test]
		public void TestRemoveTask1 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' >
						<Message Text='text' />
						<Warning Text='text' />
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);

			IEnumerator e = t [0].GetEnumerator ();
			e.MoveNext ();
			t [0].RemoveTask ((BuildTask) e.Current);
			e = t [0].GetEnumerator ();
			e.MoveNext ();
			Assert.AreEqual ("Warning", ((BuildTask) e.Current).Name, "A1");
			Assert.IsFalse (e.MoveNext (), "A2");
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void TestRemoveTask2 ()
		{
                        string documentString = @"
                                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
					<Target Name='Target' >
					</Target>
                                </Project>
                        ";

			engine = new Engine (Consts.BinPath);

                        project = engine.CreateNewProject ();
                        project.LoadXml (documentString);

			Target[] t = new Target [1];
			project.Targets.CopyTo (t, 0);
			t [0].RemoveTask (null);
		}
	}
}
