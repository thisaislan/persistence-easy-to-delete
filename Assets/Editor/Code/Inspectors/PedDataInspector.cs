using System.Collections.Generic;
using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects;
using Thisaislan.PersistenceEasyToDelete.Editor.Styles;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Inspectors
{
    [CustomEditor(typeof(PedData))]
    internal class PedDataInspector : UnityEditor.Editor
    {
        private const float RowContentHeight = 24f;
        private const float DeleteButtonWidth = 60f;
        private const float DeleteButtonHeight = 24f;
        private const float ButtonSpacing = 1f;
        private const float RowIndexWidth = 30f;
        private const float ColumnLabelWidth = 32f;
        private const float TypeFieldWidth = 130f;
        private const float MaxValueAreaLines = 5f;
        private const float ValidationCleanButtonWidth = 110f;

        private readonly List<string> validationMessages = new List<string>();

        private readonly Dictionary<int, RowProperties> playerPrefsRows = new Dictionary<int, RowProperties>();
        private readonly Dictionary<int, RowProperties> fileDataRows = new Dictionary<int, RowProperties>();

        private struct RowProperties
        {
            internal string IndexLabel;
            internal SerializedProperty Key;
            internal SerializedProperty Type;
            internal SerializedProperty Value;
        }

        private SerializedProperty isActiveProperty;
        private SerializedProperty avoidChangesProperty;
        private SerializedProperty playerPrefsProperty;
        private SerializedProperty fileDataProperty;

        private bool showPlayerPrefsData = true;
        private bool showFileData = true;
        private bool hasValidationRun;
        private bool lastValidationHasPassed;

        private void OnEnable()
        {
            isActiveProperty = serializedObject.FindProperty(Consts.IsActivePropertyName);
            avoidChangesProperty = serializedObject.FindProperty(Consts.AvoidChangesPropertyName);
            playerPrefsProperty = serializedObject.FindProperty(Consts.PlayerPrefsPropertyName);
            fileDataProperty = serializedObject.FindProperty(Consts.FileDataPropertyName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PedData pedData = (PedData)target;

            bool structuralChange = false;

            DrawStatusCard(pedData);

            DrawDuplicateActiveWarning(pedData);

            PedInspectorStyles.DrawSectionSpace();

            DrawDataSection(ref structuralChange);

            PedInspectorStyles.DrawSectionSpace();

            DrawValidationCard(pedData);

            if (serializedObject.ApplyModifiedProperties())
            {
                if (structuralChange)
                {
                    PedEditor.PersistAsset(pedData);
                }

                if (pedData.IsActivePed())
                {
                    PedEditor.SetActivePedData(pedData);
                    serializedObject.Update();
                }
            }
        }

        private void DrawStatusCard(PedData pedData)
        {
            EditorGUILayout.BeginVertical(
                PedInspectorStyles.GetCardStyle(PedInspectorStyles.SectionBackgroundColor));

            EditorGUILayout.LabelField(Consts.PedDataStatusCardLabel, EditorStyles.boldLabel);

            PedInspectorStyles.DrawLine(PedInspectorStyles.LineColor);

            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();

            bool isActive = isActiveProperty.boolValue;

            PedInspectorStyles.DrawTextButton(
                isActive ? Consts.PedDataActiveDisabledButtonLabel : Consts.PedDataActiveButtonLabel,
                PedInspectorStyles.ButtonColorStyle.Growth,
                () =>
                {
                    PedEditor.SetActivePedData(pedData);
                    serializedObject.Update();
                },
                tooltip: Consts.PedDataActiveDataTooltipAttr,
                enabled: !isActive
            );

            bool shouldAvoidChanges = avoidChangesProperty.boolValue;

            PedInspectorStyles.DrawTextButton(
                shouldAvoidChanges
                    ? Consts.PedDataAvoidAllowButtonLabel
                    : Consts.PedDataAvoidChangesButtonLabel,
                shouldAvoidChanges
                    ? PedInspectorStyles.ButtonColorStyle.Alert
                    : PedInspectorStyles.ButtonColorStyle.Neutral,
                () =>
                {
                    avoidChangesProperty.boolValue = !avoidChangesProperty.boolValue;
                },
                tooltip: Consts.PedDataAvoidChangesToggledTooltip
            );

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawDuplicateActiveWarning(PedData pedData)
        {
            bool isAnotherPedDataActive =
                pedData.IsActivePed() && PedEditor.IsAnotherPedDataActive(pedData);

            if (!isAnotherPedDataActive)
            {
                return;
            }

            EditorGUILayout.HelpBox(Consts.PedDataDuplicateActiveMessage, MessageType.Warning);

            PedInspectorStyles.DrawTextButton(
                Consts.PedDataActiveButtonLabel,
                PedInspectorStyles.ButtonColorStyle.Growth,
                () =>
                {
                    PedEditor.SetActivePedData(pedData);
                    serializedObject.Update();
                }
            );
        }

        private void DrawDataSection(ref bool structuralChange)
        {
            EditorGUILayout.BeginVertical(
                PedInspectorStyles.GetDataSectionStyle(PedInspectorStyles.SectionBackgroundColor));

            if (DrawDataListCard(
                    playerPrefsProperty,
                    ref showPlayerPrefsData,
                    Consts.PedDataPlayerPrefsSectionLabel,
                    Consts.PedDataPlayerPrefsTooltipAttr))
            {
                structuralChange = true;
            }

            DrawSectionDivider();

            if (DrawDataListCard(
                    fileDataProperty,
                    ref showFileData,
                    Consts.PedDataFileSectionLabel,
                    Consts.PedDataFileToolTipAttr))
            {
                structuralChange = true;
            }

            DrawSectionDivider();

            DrawClearAllSection(ref structuralChange);

            EditorGUILayout.EndVertical();
        }

        private void DrawSectionDivider()
        {
            PedInspectorStyles.DrawLine(PedInspectorStyles.LineColor);
            EditorGUILayout.Space(6);
        }

        private bool DrawDataListCard(
            SerializedProperty listProperty,
            ref bool isExpanded,
            string sectionTitle,
            string sectionTooltip
        )
        {
            bool changed = false;

            int listCount = listProperty.arraySize;

            EditorGUILayout.BeginVertical(
                PedInspectorStyles.GetCardStyle(PedInspectorStyles.SectionBackgroundColor));

            EditorGUILayout.BeginHorizontal();

            isExpanded = EditorGUILayout.Foldout(
                isExpanded,
                new GUIContent($"{sectionTitle} ({listCount})", sectionTooltip),
                true,
                PedInspectorStyles.BoldFoldoutStyle
            );

            GUILayout.FlexibleSpace();

            PedInspectorStyles.DrawIconButton(
                iconName: Consts.TreeEditorTrashIcon,
                style: PedInspectorStyles.ButtonColorStyle.Urgent,
                onAction: () =>
                {
                    if (EditorUtility.DisplayDialog(
                            Consts.PedDataDeleteAllDialogTitle,
                            Consts.PedDataDeleteAllDialogMessage,
                            Consts.DialogOkButton,
                            Consts.DialogCancelButton
                        ))
                    {
                        listProperty.ClearArray();
                        changed = true;
                    }
                },
                width: DeleteButtonWidth,
                height: DeleteButtonHeight,
                tooltip: Consts.PedDataDeleteAllEntriesButtonTooltip,
                enabled: listCount > 0
            );

            EditorGUILayout.EndHorizontal();

            PedInspectorStyles.DrawLine(PedInspectorStyles.LineColor);

            EditorGUILayout.Space(4);

            if (isExpanded)
            {
                DrawListContent(listProperty, ref changed);
            }

            EditorGUILayout.EndVertical();

            return changed;
        }

        private void DrawListContent(SerializedProperty listProperty, ref bool changed)
        {
            int listCount = listProperty.arraySize;

            if (listCount == 0)
            {
                EditorGUILayout.LabelField(
                    Consts.PedDataEmptyListMessage,
                    PedInspectorStyles.GetLabelStyle(PedInspectorStyles.EmptyListLabelColor),
                    GUILayout.ExpandWidth(true)
                );

                DrawListAddButton(listProperty, ref changed);

                return;
            }

            int rowToRemove = -1;

            Dictionary<int, RowProperties> rowCache = RefreshRowCache(listProperty, listCount);

            for (int index = 0; index < listCount; index++)
            {
                if (DrawDataRow(rowCache[index], index))
                {
                    rowToRemove = index;
                }
            }

            if (rowToRemove >= 0)
            {
                listProperty.DeleteArrayElementAtIndex(rowToRemove);
                changed = true;
            }

            DrawListAddButton(listProperty, ref changed);
        }

        private void DrawListAddButton(SerializedProperty listProperty, ref bool changed)
        {
            EditorGUILayout.Space(ButtonSpacing);

            bool added = PedInspectorStyles.DrawTextButton(
                Consts.PedDataAddEntryButtonLabel,
                PedInspectorStyles.ButtonColorStyle.Growth,
                onAction: null,
                tooltip: Consts.PedDataAddEntryButtonTooltip
            );

            if (!added)
            {
                return;
            }

            listProperty.InsertArrayElementAtIndex(listProperty.arraySize);

            SerializedProperty newEntry =
                listProperty.GetArrayElementAtIndex(listProperty.arraySize - 1);

            newEntry.FindPropertyRelative(Consts.KeyPropertyName).stringValue = string.Empty;
            newEntry.FindPropertyRelative(Consts.TypePropertyName).stringValue = string.Empty;
            newEntry.FindPropertyRelative(Consts.ValuePropertyName).stringValue = string.Empty;

            changed = true;
        }

        private Dictionary<int, RowProperties> RefreshRowCache(
            SerializedProperty listProperty,
            int listCount
        )
        {
            bool isPlayerPrefsList = listProperty.propertyPath == playerPrefsProperty.propertyPath;
            Dictionary<int, RowProperties> rowCache = isPlayerPrefsList ? playerPrefsRows : fileDataRows;

            if (rowCache.Count != listCount)
            {
                rowCache.Clear();

                for (int index = 0; index < listCount; index++)
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(index);

                    rowCache[index] = new RowProperties
                    {
                        IndexLabel = index + Consts.RowIndexLabelSuffix,
                        Key = element.FindPropertyRelative(Consts.KeyPropertyName),
                        Type = element.FindPropertyRelative(Consts.TypePropertyName),
                        Value = element.FindPropertyRelative(Consts.ValuePropertyName)
                    };
                }
            }

            return rowCache;
        }

        private bool DrawDataRow(RowProperties row, int index)
        {
            Color rowBackground = index % 2 == 0
                ? PedInspectorStyles.RowBackgroundColorA
                : PedInspectorStyles.RowBackgroundColorB;

            EditorGUILayout.BeginVertical(PedInspectorStyles.GetRowStyle(rowBackground));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                row.IndexLabel,
                PedInspectorStyles.GetLabelStyle(PedInspectorStyles.RowIndexLabelColor),
                GUILayout.Width(RowIndexWidth)
            );

            EditorGUILayout.LabelField(
                Consts.PedDataKeyColumnLabel,
                PedInspectorStyles.GetLabelStyle(PedInspectorStyles.RowIndexLabelColor),
                GUILayout.Width(ColumnLabelWidth)
            );

            row.Key.stringValue = EditorGUILayout.TextField(
                row.Key.stringValue,
                PedInspectorStyles.TextFieldStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(RowContentHeight)
            );

            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField(
                Consts.PedDataTypeColumnLabel,
                PedInspectorStyles.GetLabelStyle(PedInspectorStyles.RowIndexLabelColor),
                GUILayout.Width(ColumnLabelWidth)
            );

            row.Type.stringValue = EditorGUILayout.TextField(
                row.Type.stringValue,
                PedInspectorStyles.TextFieldStyle,
                GUILayout.Width(TypeFieldWidth),
                GUILayout.Height(RowContentHeight)
            );

            EditorGUILayout.Space(4);

            bool wasDeleted = PedInspectorStyles.DrawIconButton(
                Consts.TreeEditorTrashIcon,
                PedInspectorStyles.ButtonColorStyle.Urgent,
                onAction: null,
                width: RowContentHeight,
                tooltip: Consts.PedDataDeleteEntryButtonTooltip
            );

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            row.Value.stringValue = EditorGUILayout.TextArea(
                row.Value.stringValue,
                PedInspectorStyles.ValueAreaStyle,
                GUILayout.Height(ComputeValueAreaHeight(row.Value.stringValue))
            );

            EditorGUILayout.EndVertical();

            return wasDeleted;
        }

        private static float ComputeValueAreaHeight(string value)
        {
            float lineHeight = PedInspectorStyles.ValueAreaStyle.lineHeight;
            int wrappedLines = value.Contains(Consts.NewLineChar)
                ? value.Split(Consts.NewLineChar).Length
                : 1;

            float height = Mathf.Clamp(wrappedLines + 1, 1.5f, MaxValueAreaLines) * lineHeight;

            return Mathf.Ceil(height) + 6f;
        }

        private void DrawValidationCard(PedData pedData)
        {
            EditorGUILayout.BeginVertical(
                PedInspectorStyles.GetCardStyle(PedInspectorStyles.SectionBackgroundColor));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(
                Consts.PedValidationSectionLabel,
                EditorStyles.boldLabel,
                GUILayout.ExpandWidth(true)
            );

            bool isRunning = EditorApplication.isPlaying;

            PedInspectorStyles.DrawTextButton(
                Consts.PedValidationButtonLabel,
                PedInspectorStyles.ButtonColorStyle.Calm,
                () => RunValidation(pedData),
                enabled: !isRunning
            );

            if (hasValidationRun)
            {
                PedInspectorStyles.DrawTextButton(
                    Consts.PedValidationCleanButtonLabel,
                    PedInspectorStyles.ButtonColorStyle.Quiet,
                    ClearValidationMessages,
                    width: ValidationCleanButtonWidth
                );
            }

            EditorGUILayout.EndHorizontal();

            PedInspectorStyles.DrawLine(PedInspectorStyles.LineColor);

            EditorGUILayout.Space(4);

            if (isRunning)
            {
                EditorGUILayout.HelpBox(Consts.PedValidationRunningMessage, MessageType.Info);
            }

            DrawValidationMessages();

            EditorGUILayout.EndVertical();
        }

        private void RunValidation(PedData pedData)
        {
            validationMessages.Clear();
            lastValidationHasPassed = false;
            hasValidationRun = true;

            lastValidationHasPassed = pedData.IsDataValid(
                new PedData.ValidationDataErrorHandler(
                    AddValidationValueError,
                    AddValidationKeyError,
                    AddValidationTypeError
                )
            );
        }

        private void ClearValidationMessages()
        {
            validationMessages.Clear();
            hasValidationRun = false;
            lastValidationHasPassed = false;
        }

        private void DrawValidationMessages()
        {
            if (!hasValidationRun)
            {
                return;
            }

            if (lastValidationHasPassed)
            {
                EditorGUILayout.HelpBox(Consts.PedValidationSuccessMessage, MessageType.Info);
                return;
            }

            for (int index = 0; index < validationMessages.Count; index++)
            {
                EditorGUILayout.HelpBox(validationMessages[index], MessageType.Error);
            }
        }

        private void AddValidationValueError(string key, int index, bool isFileData) =>
            validationMessages.Add(
                GetValidationErrorPrefix(index, isFileData) +
                Consts.ValidationValueErrorMessage +
                key
            );

        private void AddValidationKeyError(string value, int index, bool isFileData, bool isDuplicity) =>
            validationMessages.Add(
                GetValidationErrorPrefix(index, isFileData) +
                (isDuplicity ? Consts.ValidationDuplicatedKeyErrorMessage : Consts.ValidationEmptyKeyErrorMessage) +
                value
            );

        private void AddValidationTypeError(string value, int index, bool isFileData) =>
            validationMessages.Add(
                GetValidationErrorPrefix(index, isFileData) +
                Consts.ValidationTypeErrorMessage +
                value
            );

        private static string GetValidationErrorPrefix(int index, bool isFileData) =>
            $"{Consts.DebugMessageSuffix} " +
            $"{(isFileData ? Consts.ValidationErrorMessageFileType : Consts.ValidationErrorMessagePlayerPrefsType)} " +
            $"{index} ";

        private void DrawClearAllSection(ref bool structuralChange)
        {
            bool hasAnyEntry = playerPrefsProperty.arraySize > 0 || fileDataProperty.arraySize > 0;

            bool wasCleared = false;

            PedInspectorStyles.DrawTextButton(
                Consts.PedDataClearAllButtonLabel,
                PedInspectorStyles.ButtonColorStyle.Urgent,
                () =>
                {
                    if (EditorUtility.DisplayDialog(
                            Consts.PedDataClearAllDialogTitle,
                            Consts.PedDataClearAllDialogMessage,
                            Consts.DialogOkButton,
                            Consts.DialogCancelButton
                        ))
                    {
                        playerPrefsProperty.ClearArray();
                        fileDataProperty.ClearArray();
                        wasCleared = true;
                    }
                },
                enabled: hasAnyEntry
            );

            if (wasCleared)
            {
                structuralChange = true;
            }
        }

    }
}