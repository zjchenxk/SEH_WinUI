using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SEH.Commons
{
    /// <summary>
    /// 简谱树形列表刷新消息类，继承自 ValueChangedMessage<string>，用于在不同的ViewModel之间传递字符串类型的属性更新消息。
    /// </summary>
    public class RefreshScoreListMessage : ValueChangedMessage<string>
    {
        public RefreshScoreListMessage(string value = "") : base(value)
        {

        }
    }
}
