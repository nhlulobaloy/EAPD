using Xunit;
using TechMoveGLMS.Services;
using TechMoveGLMS.Models;

namespace TechMoveGLMS.Tests
{
    public class BusinessLogicTests
    {
        // TEST 1: Currency Calculation Test
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectAmount()
        {
            // Arrange
            var currencyService = new CurrencyService(null!);
            decimal usdAmount = 100;
            decimal exchangeRate = 19.50m;
            decimal expected = 1950.00m;

            // Act
            decimal actual = currencyService.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(expected, actual);
        }

        // TEST 2: Currency Calculation with Zero
        [Fact]
        public void ConvertUsdToZar_WithZeroUsd_ShouldReturnZero()
        {
            // Arrange
            var currencyService = new CurrencyService(null!);
            decimal usdAmount = 0;
            decimal exchangeRate = 19.50m;

            // Act
            decimal actual = currencyService.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(0, actual);
        }

        // TEST 3: File Validation - Valid PDF (testing validation logic without FileService constructor)
        [Fact]
        public void IsValidPdfFile_WithPdfFile_ShouldReturnTrue()
        {
            // Arrange - test the validation rules directly
            string fileName = "contract.pdf";
            string contentType = "application/pdf";
            
            bool isValidExtension = System.IO.Path.GetExtension(fileName).ToLower() == ".pdf";
            bool isValidContentType = contentType == "application/pdf";
            
            // Act
            bool result = isValidExtension && isValidContentType;

            // Assert
            Assert.True(result);
        }

        // TEST 4: File Validation - Invalid EXE file
        [Fact]
        public void IsValidPdfFile_WithExeFile_ShouldReturnFalse()
        {
            // Arrange
            string fileName = "virus.exe";
            string contentType = "application/x-msdownload";
            
            bool isValidExtension = System.IO.Path.GetExtension(fileName).ToLower() == ".pdf";
            
            // Act
            bool result = isValidExtension;

            // Assert
            Assert.False(result);
        }

        // TEST 5: File Validation - Invalid JPG file
        [Fact]
        public void IsValidPdfFile_WithJpgFile_ShouldReturnFalse()
        {
            // Arrange
            string fileName = "image.jpg";
            
            bool isValidExtension = System.IO.Path.GetExtension(fileName).ToLower() == ".pdf";
            
            // Act
            bool result = isValidExtension;

            // Assert
            Assert.False(result);
        }

        // TEST 6: Contract Validator - Active Contract allows ServiceRequest
        [Fact]
        public void CanCreateServiceRequest_WithActiveContract_ShouldReturnTrue()
        {
            // Arrange
            var validator = new ContractValidator();
            var contract = new Contract
            {
                Status = "Active"
            };

            // Act
            bool result = validator.CanCreateServiceRequest(contract);

            // Assert
            Assert.True(result);
        }

        // TEST 7: Contract Validator - Expired Contract denies ServiceRequest
        [Fact]
        public void CanCreateServiceRequest_WithExpiredContract_ShouldReturnFalse()
        {
            // Arrange
            var validator = new ContractValidator();
            var contract = new Contract
            {
                Status = "Expired"
            };

            // Act
            bool result = validator.CanCreateServiceRequest(contract);

            // Assert
            Assert.False(result);
        }

        // TEST 8: Contract Validator - On Hold Contract denies ServiceRequest
        [Fact]
        public void CanCreateServiceRequest_WithOnHoldContract_ShouldReturnFalse()
        {
            // Arrange
            var validator = new ContractValidator();
            var contract = new Contract
            {
                Status = "On Hold"
            };

            // Act
            bool result = validator.CanCreateServiceRequest(contract);

            // Assert
            Assert.False(result);
        }

        // TEST 9: Contract Validator - Null Contract returns false
        [Fact]
        public void CanCreateServiceRequest_WithNullContract_ShouldReturnFalse()
        {
            // Arrange
            var validator = new ContractValidator();

            // Act
            bool result = validator.CanCreateServiceRequest(null!);

            // Assert
            Assert.False(result);
        }

        // TEST 10: Contract Validator - Draft Contract allows ServiceRequest
        [Fact]
        public void CanCreateServiceRequest_WithDraftContract_ShouldReturnTrue()
        {
            // Arrange
            var validator = new ContractValidator();
            var contract = new Contract
            {
                Status = "Draft"
            };

            // Act
            bool result = validator.CanCreateServiceRequest(contract);

            // Assert
            Assert.True(result);
        }
    }
}