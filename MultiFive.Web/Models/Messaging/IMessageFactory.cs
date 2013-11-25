﻿using System;

namespace MultiFive.Web.Models.Messaging
{
    public interface IMessageFactory
    {
        Message CreateJoinedMessage(Guid gameId, int receiverId);
    }
}