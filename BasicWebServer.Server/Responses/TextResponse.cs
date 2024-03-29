﻿using BasicWebServer.Server.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Responses
{
    public class TextResponse : ContentResponse
    {
        public TextResponse(string text,
            Action<Request, Response> preRenderAction = null)
            :base(text, ContentType.PlainText, preRenderAction)
        {
        }
    }
}
