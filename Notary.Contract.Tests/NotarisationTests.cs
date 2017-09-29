using Xunit;

using Notary.Common;

namespace Survey.Contract.Tests
{
    public class NotarisationTests
    {
        #region Notarise Tests

        [Fact(DisplayName = "Notarise_Successful")]
        [Trait("Contract", "Notarise")]
        public void NotariseSuccessfullyTest()
        {
            var documentHash = TestHelper.GetDocumentHash();

            var notarisationId = TestHelper.Notarise(TestHelper.TestSignatoryPublicKey, documentHash);

            Assert.NotNull(notarisationId);
            Assert.True(notarisationId.Length == Constants.NotarisationIdLengthBytes);
        }

        [Fact(DisplayName = "Notarise_Unsuccessful_Empty_Document_Hash")]
        [Trait("Contract", "Notarise")]
        public void NotariseUnsuccessfullyEmptyDocumentHashTest()
        {
            var emptyDocumentHash = new byte[0];

            var notarisationId = TestHelper.Notarise(TestHelper.TestSignatoryPublicKey, emptyDocumentHash);

            Assert.True(notarisationId == null || notarisationId.Length == 0);
        }

        [Fact(DisplayName = "Notarise_Unsuccessful_Document_Hash_Too_Small")]
        [Trait("Contract", "Notarise")]
        public void NotariseUnsuccessfullyDocumentHashTooSmallTest()
        {
            var tooSmallDocumentHash = new byte[] { 0, 1, 2, 3, 4, 5 };

            var notarisationId = TestHelper.Notarise(TestHelper.TestSignatoryPublicKey, tooSmallDocumentHash);

            Assert.True(notarisationId == null || notarisationId.Length == 0);
        }

        #endregion Notarise Tests

        #region Document Authenticity Verification Tests
        


        #endregion Document Authenticity Verification Tests
    }
}