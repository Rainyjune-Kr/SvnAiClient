using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SvnAiClient.Models;

namespace SvnAiClient.Core.AI
{
    /// <summary>AI 응답의 JSON 구조를 검증하여 검수 결과에 반영한다.</summary>
    public static class ReviewResponseParser
    {
        #region Public Methods

        /// <summary>응답 전체를 검증한 뒤 결과를 반영하며, 형식 오류는 검수 실패로 기록한다.</summary>
        public static void Parse(string text, AiReviewResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            try
            {
                var json = (text ?? string.Empty).Trim();

                // 응답 전체가 코드블록인 경우만 외곽 표기를 제거한다.
                if (json.StartsWith("```") && json.EndsWith("```"))
                {
                    int newline = json.IndexOf('\n');

                    if (newline < 0)
                    {
                        throw new JsonException(
                            "JSON 코드블록 형식이 잘못되었습니다.");
                    }

                    json = json.Substring(
                        newline + 1,
                        json.Length - newline - 4).Trim();
                }

                var root = JObject.Parse(json, new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling =
                        DuplicatePropertyNameHandling.Error
                });

                string summary = RequiredString(root, "summary");
                string commitMessage =
                    RequiredString(root, "commitMessage");

                var array = root["issues"] as JArray;

                if (array == null)
                    throw new JsonException("issues는 배열이어야 합니다.");

                var issues = new List<AiReviewIssue>();

                foreach (var token in array)
                {
                    var item = token as JObject;

                    if (item == null)
                    {
                        throw new JsonException(
                            "이슈 항목은 객체여야 합니다.");
                    }

                    string severityText =
                        RequiredString(item, "severity")
                            .ToUpperInvariant();

                    IssueSeverity severity;

                    switch (severityText)
                    {
                        case "INFO":
                            severity = IssueSeverity.Info;
                            break;

                        case "WARNING":
                            severity = IssueSeverity.Warning;
                            break;

                        case "ERROR":
                            severity = IssueSeverity.Error;
                            break;

                        default:
                            throw new JsonException(
                                "알 수 없는 severity 값입니다.");
                    }

                    var path = item["filePath"];

                    if (path != null &&
                        path.Type != JTokenType.Null &&
                        path.Type != JTokenType.String)
                    {
                        throw new JsonException(
                            "filePath는 문자열 또는 null이어야 합니다.");
                    }

                    int? lineNumber = null;
                    var line = item["lineNumber"];

                    if (line != null && line.Type != JTokenType.Null)
                    {
                        if (line.Type != JTokenType.Integer)
                        {
                            throw new JsonException(
                                "lineNumber는 정수 또는 null이어야 합니다.");
                        }

                        long number = line.Value<long>();

                        if (number < 1 || number > int.MaxValue)
                        {
                            throw new JsonException(
                                "lineNumber 범위가 잘못되었습니다.");
                        }

                        lineNumber = (int)number;
                    }

                    issues.Add(new AiReviewIssue
                    {
                        Severity = severity,
                        Category = RequiredString(item, "category"),
                        Title = RequiredString(item, "title"),
                        Description = RequiredString(item, "description"),

                        FilePath = path == null ||
                                   path.Type == JTokenType.Null
                            ? null
                            : path.Value<string>(),

                        LineNumber = lineNumber
                    });
                }

                // 모든 항목을 검증한 뒤 한 번에 반영한다.
                result.Summary = summary;
                result.SuggestedCommitMessage = commitMessage;
                result.Issues = issues;
                result.Status = AiReviewStatus.Succeeded;
                result.FailureKind = AiReviewFailureKind.None;
                result.ErrorMessage = null;
            }
            catch (Exception ex) when (
                ex is JsonException ||
                ex is FormatException ||
                ex is OverflowException)
            {
                result.Summary = string.Empty;
                result.SuggestedCommitMessage = string.Empty;
                result.Issues = new List<AiReviewIssue>();

                result.Status = AiReviewStatus.Failed;
                result.FailureKind =
                    AiReviewFailureKind.InvalidResponse;

                result.ErrorMessage =
                    "AI 응답 형식 오류: " + ex.Message;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>필수 문자열 속성을 읽고 누락되거나 형식이 다르면 오류를 전달한다.</summary>
        private static string RequiredString(
            JObject item,
            string name)
        {
            var token = item[name];

            if (token == null || token.Type != JTokenType.String)
            {
                throw new JsonException(
                    name + "는 필수 문자열입니다.");
            }

            return token.Value<string>();
        }

        #endregion
    }
}
