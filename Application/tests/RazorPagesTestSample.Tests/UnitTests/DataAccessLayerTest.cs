using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using RazorPagesTestSample.Data;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesTestSample.Tests.UnitTests
{
    public class DataAccessLayerTest
    {
        [Fact]
        public async Task GetMessagesAsync_MessagesAreReturned()
         {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expectedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(expectedMessages);
                await db.SaveChangesAsync();

                // Act
                var result = await db.GetMessagesAsync();

                // Assert
                var actualMessages = Assert.IsAssignableFrom<List<Message>>(result);
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }

        [Fact]
        public async Task AddMessageAsync_MessageIsAdded()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var recId = 10;
                var expectedMessage = new Message() { Id = recId, Text = "Message" };

                // Act
                await db.AddMessageAsync(expectedMessage);

                // Assert
                var actualMessage = await db.FindAsync<Message>(recId);
                Assert.Equal(expectedMessage, actualMessage);
            }
        }

        [Fact]
        public async Task AddMessageAsyn_MessageTextLength400_MessageIsNotAdded()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var recId = 10;
                var expectedMessage = new Message() { Id = recId, Text = new string('x', 400) };

                // Act
                try
                {
                    await db.AddMessageAsync(expectedMessage);
                }
                catch
                {
                    // Message text is too long
                }

                // Assert
                var actualMessage = await db.FindAsync<Message>(recId);
                Assert.Null(actualMessage);
            }
        }

        [Fact]
        public async Task DeleteAllMessagesAsync_MessagesAreDeleted()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var seedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(seedMessages);
                await db.SaveChangesAsync();

                // Act
                await db.DeleteAllMessagesAsync();

                // Assert
                Assert.Empty(await db.Messages.AsNoTracking().ToListAsync());
            }
        }

        [Fact]
        public async Task DeleteMessageAsync_MessageIsDeleted_WhenMessageIsFound()
         {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                #region snippet1
                // Arrange
                var seedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(seedMessages);
                await db.SaveChangesAsync();
                var recId = 1;
                var expectedMessages = 
                    seedMessages.Where(message => message.Id != recId).ToList();
                #endregion

                #region snippet2
                // Act
                await db.DeleteMessageAsync(recId);
                #endregion

                #region snippet3
                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
                #endregion
            }
        }

        #region snippet4
        [Fact]
        public async Task DeleteMessageAsync_NoMessageIsDeleted_WhenMessageIsNotFound()
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var expectedMessages = AppDbContext.GetSeedingMessages();
                await db.AddRangeAsync(expectedMessages);
                await db.SaveChangesAsync();
                var recId = 4;

                // Act
                try
                {
                    await db.DeleteMessageAsync(recId);
                }
                catch
                {
                    // recId doesn't exist
                }

                // Assert
                var actualMessages = await db.Messages.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedMessages.OrderBy(m => m.Id).Select(m => m.Text), 
                    actualMessages.OrderBy(m => m.Id).Select(m => m.Text));
            }
        }
        #endregion

        [Theory]
        [InlineData(150, true)]
        [InlineData(200, true)]
        [InlineData(225, true)]
        [InlineData(250, true)]
        [InlineData(275, false)]
        [InlineData(300, false)]
        public void Message_TextLengthIsValid(int textLength, bool expected)
        {
            using (var db = new AppDbContext(Utilities.TestDbContextOptions()))
            {
                // Arrange
                var recId = 10;
                var message = new Message() { Id = recId, Text = new string('x', textLength) };
                var validationContext = new ValidationContext(message, null, null);
                var validationResults = new List<ValidationResult>();
                
                // Act
                var actual = Validator.TryValidateObject(message, validationContext, validationResults, true);

                // Assert
                Assert.Equal(expected, actual);
            }
            
        }

    }
}

