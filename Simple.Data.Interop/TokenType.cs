using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Data.Interop
{
    public enum TokenType
    {
        NumericConstant,
        Identifier,
        OpenParen,
        CloseParen,
        Dot,
        Comma,
        StringConstant,
        LessThanSign,
        GreaterThanSign,
        EqualSign,
        BangSign,
        Colon
    }
}
