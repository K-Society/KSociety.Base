// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class QueueDeclareParameters
    {
        public bool QueueDurable { get; set; } = false;
        public bool QueueExclusive { get; set; } = false;
        public bool QueueAutoDelete { get; set; } = true;

        public QueueDeclareParameters()
        {

        }
    }
}