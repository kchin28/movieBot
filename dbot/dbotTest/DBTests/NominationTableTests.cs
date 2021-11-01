using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using dbot.Services;
using dbot.Models;
using System.Collections.Generic;

namespace dbotTest.DBTests
{
    [TestClass]
    public class NominationTableTests
    {
        [TestMethod]
        public void NominateMovie_WithEmptyDB_ShouldCallDbManagerAddNomination()
        {
            var mockUser = new Mock<Discord.IUser>();
            var mockDbManager = new Mock<dbot.Persistence.IDbManager>();
            mockDbManager.Setup(x => x.GetNominations() ).Returns(new List<Nomination>());
            mockDbManager.Setup(x => x.FindNomination(mockUser.Object)).Returns<Nomination>(null);

            var nomService = new NominationsService(mockDbManager.Object);
            nomService.AddNomination(mockUser.Object,"fakeTitle","imdbId");
            
            mockDbManager.Verify(  t => t.AddNomination(It.Is<Nomination>
            (
                n => n.VotingID==1 &&
                n.Name == "fakeTitle" &&
                n.ImdbId == "imdbId"
            )), Times.Once);
        }

        [TestMethod]
        public void NominateMovie_WithExistingNomination_ShouldCallDbManagerAddNomination()
        {
            var mockUser = new Mock<Discord.IUser>();
            var mockDbManager = new Mock<dbot.Persistence.IDbManager>();

            var nom = new Nomination() {  User = new User(mockUser.Object) };

            var dbNominations = new List<Nomination>()
            {
                nom
            };

            mockDbManager.Setup( x => x.GetNominations() ).Returns(dbNominations);
            mockDbManager.Setup(x => x.FindNomination(mockUser.Object)).Returns(nom);

            var  nomService = new NominationsService(mockDbManager.Object);
            nomService.AddNomination(mockUser.Object,"fakeTitle","imdbId");

            mockDbManager.Verify(  t => t.UpdateNomination(It.Is<Nomination>
            (
                n => 
                n.Name == "fakeTitle" &&
                n.ImdbId == "imdbId"
            )), Times.Once);

        }
    
        [TestMethod]
        public void IsNominated_WithNullNominations_ShouldReturnFalse()
        {
            var mockDbManager = new Mock<dbot.Persistence.IDbManager>();
            mockDbManager.Setup(x => x.GetNominations() ).Returns<List<Nomination>>(null);

            var nomService = new NominationsService(mockDbManager.Object);
            var result = nomService.IsNominated("imdbId");

            Assert.IsFalse(result);
        }

         [TestMethod]
        public void IsNominated_WithNoNominations_ShouldReturnFalse()
        {
            var mockDbManager = new Mock<dbot.Persistence.IDbManager>();
            mockDbManager.Setup(x => x.GetNominations() ).Returns(new List<Nomination>());

            var nomService = new NominationsService(mockDbManager.Object);
            var result = nomService.IsNominated("imdbId");

            Assert.IsFalse(result);
        }
    
    }
}
