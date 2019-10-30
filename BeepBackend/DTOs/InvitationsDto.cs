using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class InvitationsDto
    {
        public IEnumerable<InvitationListItemDto> SentInvitations { get; set; }
        public IEnumerable<InvitationListItemDto> ReceivedInvitations { get; set; }
    }
}