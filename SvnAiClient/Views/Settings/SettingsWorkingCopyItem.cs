namespace SvnAiClient.Views.Settings
{
    /// <summary>비밀번호 없이 작업 복사본 목록을 표시하기 위한 화면 데이터.</summary>
    public sealed class SettingsWorkingCopyItem
    {
        #region Properties

        /// <summary>편집용 설정 목록에서의 원래 인덱스. 표를 정렬해도 같은 항목을 선택한다.</summary>
        public int Index { get; set; }

        /// <summary>목록에 표시할 작업 복사본 이름.</summary>
        public string Name { get; set; }

        /// <summary>작업 복사본의 로컬 경로.</summary>
        public string LocalPath { get; set; }

        /// <summary>SVN 인증 사용자 이름.</summary>
        public string Username { get; set; }

        #endregion
    }
}
