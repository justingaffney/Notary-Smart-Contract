using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

using Notary.Common;

namespace Notary.Contract
{
    /// <summary>
    ///     A smart contract that allows documents to be notarised.
    /// </summary>
    public class MainContract : SmartContract
    {
        /// <summary>
        ///     Entry point of the smart contract.
        /// </summary>
        /// <param name="operation">
        ///     Name of the requested operation.
        /// </param>
        /// <param name="args">
        ///     Operation arguments.
        /// </param>
        public static byte[] Main(string operation, params object[] args)
        {
            if (operation == Operations.NOTARISE)
            {
                return Notarise((byte[]) args[0], (byte[]) args[1]);
            }
            else if (operation == Operations.GETDETAILS)
            {
                return GetNotarisationDetails((byte[]) args[0]);
            }

            else // Invalid operation
            {
                return new byte[0];
            }
        }

        /// <summary>
        ///     Notarises a document by registering its cryptographic hash and the signatory's
        ///     public key in the contract storage so the document can be verified later.
        /// </summary>
        /// <param name="signatory">
        ///     Document signatory's public key.
        /// </param>
        /// <param name="documentHash">
        ///     Cryptographic hash of the document being notarised.
        /// </param>
        /// <returns>
        ///     If notarisation is unsuccessful an empty byte array is returned.
        ///     
        ///     If notarisation is successful a unique 20-byte notarisation identifier is returned
        ///     that allows a document's authenticity to be verified later.
        /// </returns>
        private static byte[] Notarise(byte[] signatory, byte[] documentHash)
        {
            var emptyBytes = new byte[0];
            
            // Validate operation parameters
            if (signatory == null || signatory.Length != Constants.PublicKeyLengthBytes) return emptyBytes;

            if (documentHash == null || documentHash.Length < Constants.MinimumDocumentHashLengthBytes) return emptyBytes;


            // Verify signatory chose to register document hash
            if (!Runtime.CheckWitness(signatory)) return emptyBytes;


            // Encode notarisation details
            var notarisationDetails = EncodeNotarisationDetails(Blockchain.GetHeight(), signatory, documentHash);


            // Generate notarisation identifier
            var notarisationId = Sha1(notarisationDetails);


            // Store encoded notarisation details in contract storage
            Storage.Put(Storage.CurrentContext, notarisationId, notarisationDetails);


            // Document has been successfully notarised
            return notarisationId;
        }

        /// <summary>
        /// 
        ///     Gets the encoded notarisation details. The encoded byte array contains the block
        ///     height of when the document was notarised, the public key of the document's
        ///     signatory, and the notarised document's registered cryptographic hash.
        ///     
        ///     The encoding scheme is:
        ///     
        ///         0 - 3             4 - 35                   36 - *
        ///     [Block Height] [Signatory Public Key] [Registered Document Hash]
        /// 
        /// </summary>
        /// <param name="notarisationId">
        ///     Unique notarisation identifier.
        /// </param>
        /// <returns>
        ///     Byte array containing encoded notarisation details, or an empty byte array if the
        ///     notarisation identifier is invalid.
        /// </returns>
        private static byte[] GetNotarisationDetails(byte[] notarisationId)
        {
            var emptyBytes = new byte[0];
            
            // Validate notarisation identifier
            if (notarisationId == null || notarisationId.Length != Constants.NotarisationIdLengthBytes) return emptyBytes;

            
            // Get encoded notarisation details
            var notarisationDetails = Storage.Get(Storage.CurrentContext, notarisationId);
            
            if (notarisationDetails == null) return emptyBytes;

            return notarisationDetails;
        }


        #region Encoding Methods

        /// <summary>
        /// 
        ///     Encodes the notarisation details. The encoding scheme is:
        ///     
        ///         0 - 3             4 - 35                   36 - *
        ///     [Block Height] [Signatory Public Key] [Registered Document Hash]
        /// 
        /// </summary>
        /// <param name="height">
        ///     The height of the current block.
        /// </param>
        /// <param name="signatory">
        ///     Document signatory's public key.
        /// </param>
        /// <param name="documentHash">
        ///     Cryptographic hash of the document being notarised.
        /// </param>
        /// <returns>
        ///     Byte array containing encoded notarisation details.
        /// </returns>
        private static byte[] EncodeNotarisationDetails(uint height, byte[] signatory, byte[] documentHash)
        {
            return AsByteArray(height).Concat(signatory).Concat(documentHash);
        }

        /// <summary>
        ///     Encodes an unsigned integer into a byte array using little-endian.
        /// </summary>
        /// <param name="source">
        ///     The unsigned integer to encode.
        /// </param>
        /// <returns>
        ///     The little-endian encoded unsigned integer.
        /// </returns>
        private static byte[] AsByteArray(uint source)
        {
            var firstByte = (byte)(source & 0xFF);
            var secondByte = (byte)((source >> 8) & 0xFF);
            var thirdByte = (byte)((source >> 16) & 0xFF);
            var fourthByte = (byte)((source >> 24) & 0xFF);

            return new byte[]
            {
                firstByte,
                secondByte,
                thirdByte,
                fourthByte
            };
        }

        #endregion Encoding Methods
    }
}