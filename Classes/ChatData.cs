using KickLib.Client.Models.Args;
using KickLib.Client.Models.Events.Chatroom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTSBot;
using TwitchLib.Api.Core.Models.Undocumented.Comments;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TTSBot;
public class ChatData
    {
    public string user_name {  get; set; }  
    public string message { get; set; }
    public string channel { get; set; }
    public string origin { get; set; }
    public bool is_subscriber { get; set; }
    public Commands.UserLevel user_level { get; set; }

    public ChatData(OnMessageReceivedArgs e, SettingsManager settingsManager = null)
    {
        // twitch data process to generic chat message type
        this.user_name = e.ChatMessage.Username;
        this.channel = e.ChatMessage.Channel;
        this.message = e.ChatMessage.Message;
        this.origin = "TWITCH";
        this.is_subscriber = e.ChatMessage.IsSubscriber;
        this.user_level = Commands.UserLevel.USER;

        if (e.ChatMessage.IsVip)
        {
            this.user_level = Commands.UserLevel.VIP;
        }

        if (e.ChatMessage.IsModerator)
        {
            this.user_level = Commands.UserLevel.MOD;
        }

        if (e.ChatMessage.IsBroadcaster)
        {
            this.user_level = Commands.UserLevel.STREAMER;
        }

        if (settingsManager != null)
        {

            if (e.ChatMessage.Username.ToLower() == settingsManager.settings.twitchAdminUserName)
            {
                this.user_level = Commands.UserLevel.ADMIN;
            }

        }

    }

    public ChatData(ChatMessageEventArgs e, SettingsManager settingsManager = null)
    {

        // kick data process to generic chat message type
        this.user_name = e.Data.Sender.Username;
        this.channel = e.Data.ChatroomId.ToString();
        this.message = e.Data.Content;
        this.origin = "KICK";
        // need to look at badges for this
        this.is_subscriber = false;
        // need to look at badges for this
        this.user_level = Commands.UserLevel.USER;
        ICollection<Badge> badges = e.Data.Sender.Identity.Badges;
        foreach (Badge badge in badges) { 

        if (badge.Type.ToLower() == "vip")
            {
                if (this.user_level < Commands.UserLevel.VIP)
                {
                    this.user_level = Commands.UserLevel.VIP;
                }
            }
        if (badge.Type.ToLower() == "moderator")
            {
                if (this.user_level < Commands.UserLevel.MOD)
                {
                    this.user_level = Commands.UserLevel.MOD;
                }
            }

        if (badge.Type.ToLower() == "broadcaster")
            {
                this.user_level = Commands.UserLevel.STREAMER;
            }

        }
        if (settingsManager != null)
        {
            if (user_name.ToLower() == settingsManager.settings.kickChannelAdminUserName.ToLower())
            {
                this.user_level = Commands.UserLevel.ADMIN;
            }
        }

    }


    }

