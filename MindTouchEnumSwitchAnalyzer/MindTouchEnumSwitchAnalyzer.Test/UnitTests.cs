﻿/**
 * Copyright (c) 2016 MindTouch Inc.
 * www.mindtouch.com  oss@mindtouch.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace MindTouchEnumSwitchAnalyzer.Test {

    [TestClass]
    public class UnitTest : CodeFixVerifier {

        [TestMethod]
        public void No_diagnostics_when_all_cases_covered() {
            var test = @"
    using System;
    namespace Application {
        
        enum MyEnum { A, B, C, D, E, F };

        class MyClass {
            public static void Function() {
                MyEnum e;
                switch(e) {
                    case MyEnum.A:
                    case MyEnum.B:
                    case MyEnum.C:
                    case MyEnum.D:
                    case MyEnum.E:
                    case MyEnum.F:
                        break;
                }
            }
        }    
    }";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void Diagnostics_and_code_fix_when_enum_value_missing() {
            var test = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e) 
            {
                case MyEnum.A:
                case MyEnum.B:
                case MyEnum.C:
                case MyEnum.D:
                case MyEnum.E:
                    break;
            }
        }
    }
}";
            var expected = new DiagnosticResult {
                Id = "EnumSwitchAnalyzer",
                Message = string.Format("switch on enum 'MyEnum' is missing the following members: 'F'"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };
            VerifyCSharpDiagnostic(test, expected);
            var fixtest = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e)
            {
                case MyEnum.A:
                case MyEnum.B:
                case MyEnum.C:
                case MyEnum.D:
                case MyEnum.E:
                    break;
                case MyEnum.F:
                    throw new NotImplementedException();
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void Diagnostics_and_code_fix_when_enum_value_missing_with_default() {
            var test = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e) 
            {
                case MyEnum.A:
                case MyEnum.B:
                case MyEnum.C:
                case MyEnum.D:
                case MyEnum.E:
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}";
            var expected = new DiagnosticResult {
                Id = "EnumSwitchAnalyzer",
                Message = string.Format("switch on enum 'MyEnum' is missing the following members: 'F'"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };
            VerifyCSharpDiagnostic(test, expected);
            var fixtest = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e)
            {
                case MyEnum.A:
                case MyEnum.B:
                case MyEnum.C:
                case MyEnum.D:
                case MyEnum.E:
                    break;
                case MyEnum.F:
                    throw new NotImplementedException();
                default:
                    throw new Exception();
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void Diagnostics_and_code_fix_when_switch_is_empty() {
            var test = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e)
            {
            }
        }
    }
}";
            var expected = new DiagnosticResult {
                Id = "EnumSwitchAnalyzer",
                Message = string.Format("switch on enum 'MyEnum' is missing the following members: 'A, B, C, D, E, F'"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 10, 13)
                        }
            };
            VerifyCSharpDiagnostic(test, expected);
            var fixtest = @"
    using System;
    namespace Application {

    enum MyEnum { A, B, C, D, E, F }

    class MyClass {
        public static void Function() {
            MyEnum e = 0;
            switch (e)
            {
                case MyEnum.A:
                    throw new NotImplementedException();
                case MyEnum.B:
                    throw new NotImplementedException();
                case MyEnum.C:
                    throw new NotImplementedException();
                case MyEnum.D:
                    throw new NotImplementedException();
                case MyEnum.E:
                    throw new NotImplementedException();
                case MyEnum.F:
                    throw new NotImplementedException();
            }
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider() {
            return new EnumSwitchAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() {
            return new EnumSwitchAnalyzerAnalyzer();
        }
    }
}