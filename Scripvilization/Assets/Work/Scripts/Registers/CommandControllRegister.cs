using Assets.Work.Scripts.Core.Finders;
using Assets.Work.Scripts.Executors;
using MethodArchiveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static Assets.Work.Scripts.Executors.CommandExecutor;

namespace Assets.Work.Scripts.Registers
{
    public class CommandControllRegister : MonoBehaviour, IRegister
    {
        private const int IF_CONDITION_BACK_COUNT = 4;
        private const string INVALID_CONDITION_ARG_MESSAGE = "The argument for the start of the condition is invalid.";

        [Serializable]
        private class JumpDirectionInfo
        {
            public string keyword;
            public bool isUpKeyword;
        }

        [Header("Default Setting")]
        [SerializeField] private ObjectFinderSO commandExecutorFinder;

        [Header("Command Setting")]
        [SerializeField] private string[] conditionTrueKeywords;
        [SerializeField] private string[] conditionFalseKeywords;
        [SerializeField] private string[] conditionSkipKeywords;
        [SerializeField] private JumpDirectionInfo[] conditionJumpDirectionKeywords;

        private static CommandExecutor _commandExecutor;

        // I want to implement strong constraints by using precise keywords because the objective is programming learning.
        // 프로그래밍 학습이 목적이므로 정확한 키워드를 사용하는 강한 제약을 주고자 함.
        private static HashSet<string> _conditionTrueKeywords;
        private static HashSet<string> _conditionFalseKeywords;
        private static HashSet<string> _conditionSkipKeywords;
        // true = up, false = down
        private static Dictionary<string, bool> _conditionJumpDirectionKeywords;

        private void Start()
        {
            _commandExecutor = commandExecutorFinder.GetObject<CommandExecutor>();

            _conditionTrueKeywords = new HashSet<string>();
            _conditionFalseKeywords = new HashSet<string>();
            _conditionSkipKeywords = new HashSet<string>();
            _conditionJumpDirectionKeywords = new Dictionary<string, bool>();

            foreach (string condition in conditionTrueKeywords)
                _conditionTrueKeywords.Add(condition);

            foreach (string condition in conditionFalseKeywords)
                _conditionFalseKeywords.Add(condition);

            foreach (string condition in conditionSkipKeywords)
                _conditionSkipKeywords.Add(condition);

            foreach (JumpDirectionInfo condition in conditionJumpDirectionKeywords)
                _conditionJumpDirectionKeywords.TryAdd(condition.keyword, condition.isUpKeyword);
        }

        // 만약 | (감지 나 벽 앞) ({이}면/아니면) (건너뛰기 위로 5줄)
        [ArchiveMethod("만약")]
        public static ProgramCounterMoveInformation IFConditionalStatement(string[] parameters)
        {
            bool condition;
            int moveValue;
            string conditionKeywordCommand;
            string[] parseToParameters;

            List<string> lappingList = parameters.ToList();

            // 조건문 필요 키워드들 제외하고 조건이 필요하니까
            int startIndex = lappingList.Count - IF_CONDITION_BACK_COUNT;
            if (startIndex <= 0)
            {
                // if
                // command 꼴이 될 수 있게
                condition = false;
                moveValue = 1;
            }
            // 점핑문이 입력되어있으면
            else
            {
                // 조건문에 반드시 필요한 부분을 가져옴.
                string[] ifConditionParam = lappingList.GetRange(startIndex, IF_CONDITION_BACK_COUNT).ToArray();
                lappingList.RemoveRange(startIndex, IF_CONDITION_BACK_COUNT);

                // index == 0 : 만약 조건문의 참/거짓 부분이 키워드와 매칭이 안 되면 예외발생
                condition = _conditionTrueKeywords.Contains(ifConditionParam[0]) ? true
                                : _conditionFalseKeywords.Contains(ifConditionParam[0]) ? false
                                : throw new FormatException($"{INVALID_CONDITION_ARG_MESSAGE} : condition keyword is invalid.");

                // index == 1 : 만약 조건문의 점핑 부분이 키워드와 매칭이 안 되면 예외발생
                if (!_conditionSkipKeywords.Contains(ifConditionParam[1]))
                    throw new FormatException($"{INVALID_CONDITION_ARG_MESSAGE} : jump keyword is invalid.");

                // index == 2가 방향이라 효율을 위해 3 -> 2 숫서로 작성함.
                // index == 3 : 조건문의 이동 명령어 수
                Match match = Regex.Match(ifConditionParam[3], @"^\d+");
                if (match.Success)
                    moveValue = int.Parse(match.Value);
                else
                    throw new FormatException($"{INVALID_CONDITION_ARG_MESSAGE} : move command keyword is invalid.");

                // index == 2 : 만약 조건문의 점핑 방향 부분이 키워드와 매칭이 안 되면 예외발생
                if (_conditionJumpDirectionKeywords.TryGetValue(ifConditionParam[2], out bool direction))
                    // for -> i = 0; -> 명령어 실행 -> i - 1 -> i = 0
                    // 즉 뒤로 가는 경우는 더 해지는 구조를 생각해서 -1더 해줘야함. (고치긴 해야하는데 for같은 반복 안 돌며 고쳐야해서 시간이 오래걸릴듯.)
                    moveValue = direction ? moveValue : -moveValue - 1;
                else
                    throw new FormatException($"{INVALID_CONDITION_ARG_MESSAGE} : jump direction keyword is invalid.");
            }

            // 조건의 커맨드 키워드를 가져와서
            conditionKeywordCommand = lappingList[0];
            lappingList.RemoveAt(0);
            // 나머지 string을 넘김.
            parseToParameters = lappingList.ToArray();
            ReturnValue returnValue = _commandExecutor.InvokeCommand(conditionKeywordCommand, parseToParameters);

            if (returnValue is not null &&
                returnValue.value is bool success)
                return new ProgramCounterMoveInformation().Init(success == condition ? moveValue : 0);

            throw new FormatException(INVALID_CONDITION_ARG_MESSAGE);
        }
    }
}
