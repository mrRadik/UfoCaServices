﻿namespace SystemFacade;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}