using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Game.Domain
{
    public class GameTurnEntity
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public int TurnIndex { get; set; }
        public Guid WinnerId { get; set; }
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<Guid, PlayerDecision> Decisions { get; set; }
    }
}