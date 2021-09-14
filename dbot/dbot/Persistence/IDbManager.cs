using System.Collections.Generic;
using dbot.Models;
using Discord;

namespace dbot.Persistence
{
    public interface IDbManager
    {
        Nomination FindNomination(int movieId);
        Vote FindVote(IUser user);
        void AddVote(Vote newVote);
        void UpdateNominationInVote(Vote vote, Nomination newNomination);
        List<Session> GetSessions();
        Session GetCurrentSession();
        void AddSession(Session newSession);
        void UpdateSession(Session updatedSession);
        int GetVotesForNomination(Nomination nomination);

        List<User> GetVoters();
        void ClearVotes();
    }
}