using System.Collections.Generic;
using dbot.Models;
using Discord;

namespace dbot.Persistence
{
    public interface IDbManager
    {
        User FindUser(IUser user);
        void AddUser(User user);
        Nomination FindNomination(int movieId);
        Nomination FindNomination(IUser user);
        Vote FindVote(IUser user);
        void AddVote(Vote newVote);
        void UpdateNominationInVote(Vote vote);
        List<Session> GetSessions();
        Session GetCurrentSession();
        void AddSession(Session newSession);
        void UpdateSession(Session updatedSession);
        int GetVotesForNomination(Nomination nomination);
        List<User> GetVoters();
        void ClearVotes();
        void AddNomination(Nomination newNom);
        void UpdateNomination(Nomination currNom);
        List<Nomination> GetNominations();
        void ClearNominations();
        Nomination GetNomination(IUser user);
        void DeleteNomination(Nomination nom);
        void UpdateVotingIDs(IEnumerable<Nomination> nominations);
    }
}