using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        void Insert(GameTurnEntity gameTurnEntity);
        IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId, int count);
    }
}