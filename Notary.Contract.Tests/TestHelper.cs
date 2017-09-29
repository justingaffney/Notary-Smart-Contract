using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

using Neo.Cryptography;
using Neo.IO;
using Neo.SmartContract;
using Neo.VM;

using Notary.Common;

namespace Survey.Contract.Tests
{
    internal static class TestHelper
    {
        private const string NotaryContractFilePath = @"Notary.Contract.avm";

        private const string TestNotarisationDocumentPath = "document.txt";

        internal static readonly byte[] TestSignatoryPublicKey = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

        internal static byte[] Notarise(byte[] signatory, byte[] documentHash)
        {
            using (var engine = LoadContractScript())
            {
                // Get arguments script
                var argumentsScript = GetArgumentsScript(Operations.NOTARISE, signatory, documentHash);

                // Load arguments script into engine
                engine.LoadScript(argumentsScript);

                // Start execution
                engine.Execute();

                // Get result
                var result = engine.EvaluationStack.Peek();

                // TODO Get notarisation identifier
                var notarisationId = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                
                // Return notarisation identifier
                return notarisationId;
            }
        }

        internal static byte[] GetDocumentHash()
        {
            // TODO Implement

            return new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        #region Script Helper Methods

        private static ExecutionEngine LoadContractScript()
        {
            var engine = new ExecutionEngine(null, Crypto.Default);

            // Load smart contract script into engine
            var contractScriptBytes = File.ReadAllBytes(NotaryContractFilePath);

            engine.LoadScript(contractScriptBytes);

            return engine;
        }

        private static byte[] GetArgumentsScript(string operationName, params object[] args)
        {
            Debug.Assert(operationName != null && args != null);

            using (var builder = new ScriptBuilder())
            {
                // Last argument is pushed first
                foreach (var arg in args.Reverse())
                {
                    EmitPush(builder, arg);
                }

                // Push operation name last as it is the first argument
                builder.EmitPush(operationName);

                return builder.ToArray();
            }
        }

        private static void EmitPush(ScriptBuilder builder, object arg)
        {
            if (arg is bool)
            {
                builder.EmitPush((bool)arg);
            }
            else if (arg is byte[])
            {
                builder.EmitPush((byte[])arg);
            }
            else if (arg is string)
            {
                builder.EmitPush((string)arg);
            }
            else if (arg is BigInteger)
            {
                builder.EmitPush((BigInteger)arg);
            }
            else if (arg is ContractParameter)
            {
                builder.EmitPush((ContractParameter)arg);
            }
            else if (arg is ISerializable)
            {
                builder.EmitPush((ISerializable)arg);
            }
            else
            {
                builder.EmitPush(arg);
            }
        }

        #endregion Script Helper Methods
    }
}