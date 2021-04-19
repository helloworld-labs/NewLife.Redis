﻿using System;
using NewLife.Caching.Common;
using NewLife.Log;

namespace NewLife.Caching
{
    /// <summary>消息队列基类</summary>
    public abstract class QueueBase : RedisBase
    {
        #region 属性
        /// <summary>追踪名。默认Key，主要用于解决动态Topic导致产生大量埋点的问题</summary>
        public String TraceName { get; set; }

        /// <summary>是否在消息报文中自动注入TraceId。TraceId用于跨应用在生产者和消费者之间建立调用链，默认true</summary>
        public Boolean AttachTraceId { get; set; } = true;

        /// <summary>失败时抛出异常。默认false</summary>
        public Boolean ThrowOnFailure { get; set; } = false;

        /// <summary>发送消息失败时的重试次数。默认3次</summary>
        public Int32 RetryTimesWhenSendFailed { get; set; } = 3;

        /// <summary>重试间隔。默认1000ms</summary>
        public Int32 RetryInterval { get; set; } = 1000;

        /// <summary>消息队列主题</summary>
        public String Topic => Key;
        #endregion

        #region 构造
        /// <summary>实例化延迟队列</summary>
        /// <param name="redis"></param>
        /// <param name="key"></param>
        public QueueBase(Redis redis, String key) : base(redis, key) => TraceName = key;
        #endregion

        #region 方法
        /// <summary>验证失败</summary>
        /// <param name="span"></param>
        protected void ValidWhenSendFailed(ISpan span)
        {
            if (ThrowOnFailure)
            {
                var ex = new RedisException($"发布到队列[{Topic}]失败！");
                span?.SetError(ex, null);
                throw ex;
            }
        }
        #endregion
    }
}