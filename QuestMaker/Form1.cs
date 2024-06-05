using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace QuestMaker
{
    public partial class Form1 : Form
    {
        private readonly List<object> conditions = new List<object>();
        private string[] savageRole;
        private string[] bodyPart;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddCurrentCondition();
            UpdateJsonPreview();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            RemoveLastCondition();
            UpdateJsonPreview();
        }

        private void AddCurrentCondition()
        {
            // Add conditions based on different group box visibility
            // Example for HandoverItem condition type
            if (gbHandoverItem.Visible)
            {
                conditions.Add(new
                {
                    conditionType = "HandoverItem",
                    dogtagLevel = int.TryParse(txtDogtagLevel.Text, out var dogtagLevel) ? dogtagLevel : 0,
                    dynamicLocale = chkDynamicLocale.Checked,
                    globalQuestCounterId = txtGlobalQuestCounterId.Text,
                    id = $"{txtID.Text} AFF_{conditions.Count}",
                    index = int.TryParse(txtIndex.Text, out var index) ? index : 0,
                    maxDurability = int.TryParse(txtMaxDurability.Text, out var maxDurability) ? maxDurability : 100,
                    minDurability = int.TryParse(txtMinDurability.Text, out var minDurability) ? minDurability : 0,
                    onlyFoundInRaid = chkOnlyFoundInRaid.Checked,
                    parentId = txtParentId.Text,
                    target = new[] { txtTarget.Text },
                    value = int.TryParse(txtValue.Text, out var value) ? value : 0,
                    visibilityConditions = new string[] { }
                });
            }

            if (cmbConditionType.SelectedItem.ToString() == "CounterCreator")
            {
                var counter = new
                {
                    completeInSeconds = int.TryParse(txtCompleteInSeconds.Text, out var completeInSeconds) ? completeInSeconds : 0,
                    conditionType = "CounterCreator",
                    counter = new
                    {
                        conditions = new object[]
                        {
                            new
                            {
                                bodyPart = CBbodyPart.Checked ? bodyPart : new string[] { }, // Use the bodyPary variable here
                                compareMethod = txtCompareMethodCounter.Text,
                                conditionType = "Kills",
                                daytime = new
                                {
                                    from = int.TryParse(txtDaytimeFrom.Text, out var daytimeFrom) ? daytimeFrom : 0,
                                    to = int.TryParse(txtDaytimeTo.Text, out var daytimeTo) ? daytimeTo : 0
                                },
                                distance = new
                                {
                                    compareMethod = txtDistanceCompareMethod.Text,
                                    value = int.TryParse(txtDistanceValue.Text, out var distanceValue) ? distanceValue : 0
                                },
                                dynamicLocale = chkDynamicLocaleCounter.Checked,
                                enemyEquipmentExclusive = new string[] { txtEnemyEquipmentExclusive.Text },
                                enemyEquipmentInclusive = new string[] { txtEnemyEquipmentInclusive.Text },
                                enemyHealthEffects = new string[] { txtEnemyHealthEffects.Text },
                                id = $"{txtID.Text} AFF_{conditions.Count}_0",
                                resetOnSessionEnd = chkResetOnSessionEnd.Checked,
                                savageRole = CBsavRole.Checked ? savageRole : new string[] { },
                                target = txtTargetCounter.Text,
                                value = int.TryParse(txtValueCounter.Text, out var valueCounter) ? valueCounter : 0,
                                weapon = new string[] { txtWeapon.Text },
                                weaponCaliber = new string[] { txtWeaponCaliber.Text },
                                weaponModsExclusive = new string[] { txtWeaponModsExclusive.Text },
                                weaponModsInclusive = new string[] { txtWeaponModsInclusive.Text }
                            },
                            new
                            {
                                conditionType = "Location",
                                dynamicLocale = chkDynamicLocaleLocation.Checked,
                                id = $"{txtID.Text} AFF_{conditions.Count}_1",
                                target = new string[] { txtLocationTarget.Text }
                            }
                        },
                        id = $"{txtID.Text} AFF_{conditions.Count}_2"
                    },
                    doNotResetIfCounterCompleted = chkDoNotResetIfCounterCompleted.Checked,
                    dynamicLocale = chkDynamicLocaleCondition.Checked,
                    globalQuestCounterId = txtGlobalQuestCounterIdCondition.Text,
                    id = $"{txtID.Text} AFF_0",
                    index = int.TryParse(txtIndexCondition.Text, out var indexCondition) ? indexCondition : 0,
                    oneSessionOnly = chkOneSessionOnly.Checked,
                    parentId = txtParentIdCondition.Text,
                    type = "Elimination",
                    value = int.TryParse(txtValueCondition.Text, out var valueCondition) ? valueCondition : 0,
                    visibilityConditions = new string[] { txtVisibilityConditionsCondition.Text }
                };

                conditions.Add(counter);
            }
        }

        private void CBsavRole_CheckedChanged(object sender, EventArgs e)
        {
            savageRole = new string[] { txtSavageRole.Text };
        }

        private void CBbodyPart_CheckedChanged(object sender, EventArgs e)
        {
                bodyPart = new string[] { txtBodyPart.Text };
        }

        private void RemoveLastCondition()
        {
            if (conditions.Any())
            {
                conditions.RemoveAt(conditions.Count - 1);
            }
        }

        private void ControlValueChanged(object sender, EventArgs e)
        {
            UpdateJsonPreview();
        }

        private void CmbConditionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAllGroupBoxes();

            switch (cmbConditionType.SelectedItem.ToString())
            {
                case "HandoverItem":
                    gbHandoverItem.Visible = true;
                    txtCompareMethod.Enabled = false;
                    labCompareMethod.Enabled = false;
                    GBCounterCreator.Visible = false;
                    break;
                case "FindItem":
                    gbFindItem.Visible = true;
                    txtCompareMethod.Enabled = false;
                    labCompareMethod.Enabled = false;
                    GBCounterCreator.Visible = false;
                    break;
                case "Skill":
                    gbSkill.Visible = true;
                    txtCompareMethod.Enabled = true;
                    labCompareMethod.Enabled = true;
                    GBCounterCreator.Visible = false;
                    break;
                case "LeaveItemAtLocation":
                    gbLeaveItemAtLocation.Visible = true;
                    txtCompareMethod.Enabled = false;
                    labCompareMethod.Enabled = false;
                    GBCounterCreator.Visible = false;
                    break;
                case "PlaceBeacon":
                    gbPlaceBeacon.Visible = true;
                    txtCompareMethod.Enabled = false;
                    labCompareMethod.Enabled = false;
                    GBCounterCreator.Visible = false;
                    break;
                case "CounterCreator":
                    GBCounterCreator.Visible = true;
                    gbPlaceBeacon.Visible = false;
                    txtCompareMethod.Enabled = false;
                    labCompareMethod.Enabled = false;
                    break;
            }

            UpdateJsonPreview();
        }

        private void CmbCounterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideAllGroupBoxes();

            switch (CmbCounterType.SelectedItem.ToString())
            {
                case "Kills":
                    GBkills.Visible = true;
                    GBCounterCreator.Visible = true;
                    break;
                case "VisitPlace":
                    GBkills.Visible = false;
                    GBCounterCreator.Visible = true;
                    break;
                case "Location":
                    GBkills.Visible = false;
                    GBCounterCreator.Visible = true;
                    break;
            }

            UpdateJsonPreview();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            UpdateJsonPreview();

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, rtbJsonPreview.Text);
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Quest_panel.Visible = true;
            rtbJsonPreview.Clear();
            rtbNumbers.Clear();
        }

        private void BtnBuildWeapon_Click(object sender, EventArgs e)
        {
            Quest_panel.Visible = false;
            rtbJsonPreview.Clear();
            rtbNumbers.Clear();
        }

        private void HideAllGroupBoxes()
        {
            gbHandoverItem.Visible = false;
            gbFindItem.Visible = false;
            gbSkill.Visible = false;
            gbLeaveItemAtLocation.Visible = false;
            gbPlaceBeacon.Visible = false;
            GBCounterCreator.Visible = false;
            GBkills.Visible = false;
        }

        private void UpdateJsonPreview()
        {
            rtbJsonPreview.Clear();
            rtbNumbers.Clear();

            var questData = new Dictionary<string, object>
            {
                [txtQuest.Text] = new
                {
                    QuestName = txtQuestName.Text,
                    _id = txt_ID.Text,
                    acceptPlayerMessage = $"{txtQuest.Text} acceptPlayerMessage",
                    canShowNotificationsInGame = chkCanShowNotificationsInGame.Checked,
                    changeQuestMessageText = $"{txtQuest.Text} changeQuestMessageText",
                    completePlayerMessage = $"{txtQuest.Text} completePlayerMessage",
                    conditions = new
                    {
                        AvailableForFinish = conditions
                    }
                }
            };

            var json = JsonConvert.SerializeObject(questData, Formatting.Indented);
            var lines = json.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (var i = 0; i < lines.Length; i++)
            {
                rtbJsonPreview.AppendText(lines[i] + Environment.NewLine);
                rtbNumbers.AppendText((i + 1).ToString().PadLeft(4) + "  " + Environment.NewLine);
            }
        }

        private void RtbJsonPreview_VScroll(object sender, EventArgs e)
        {
            SyncScroll(rtbJsonPreview, rtbNumbers);
        }

        private void SyncScroll(RichTextBox source, RichTextBox target)
        {
            const int WM_USER = 0x400;
            const int EM_GETSCROLLPOS = WM_USER + 221;
            const int EM_SETSCROLLPOS = WM_USER + 222;

            var scrollPos = new POINT();
            SendMessage(source.Handle, EM_GETSCROLLPOS, IntPtr.Zero, ref scrollPos);
            SendMessage(target.Handle, EM_SETSCROLLPOS, IntPtr.Zero, ref scrollPos);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, ref POINT lParam);

        private void BtnItemFinder_Click(object sender, EventArgs e)
        {
            var url = "https://db.sp-tarkov.com/search";
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnQuestID_Click(object sender, EventArgs e)
        {
            var url = "https://hub.sp-tarkov.com/doc/entry/82-updated-quest-id-list/";
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnWeaponParts_Click(object sender, EventArgs e)
        {
            var url = "https://hub.sp-tarkov.com/doc/entry/79-resources-item-id-groupings-weapons-and-weapon-parts/";
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsbtnColor_Click(object sender, EventArgs e)
        {
            // Open a color dialog to choose a color
            using (var colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Change the selection color in the RichTextBox
                    rtbJsonPreview.ForeColor = colorDialog.Color;
                }
            }
        }

        private void TsbtnFont_Click(object sender, EventArgs e)
        {
            // Open a font dialog to choose a font
            using (var fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    // Change the selection font in the RichTextBox
                    rtbJsonPreview.Font = fontDialog.Font;
                }
            }
        }

        private void TsbtnBold_Click(object sender, EventArgs e)
        {
            var currentFont = rtbJsonPreview.SelectionFont;
            var newFontStyle = currentFont.Bold ? FontStyle.Regular : FontStyle.Bold;
            rtbJsonPreview.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
        }

        private void TsbtnItalic_Click(object sender, EventArgs e)
        {
            var currentFont = rtbJsonPreview.SelectionFont;
            var newFontStyle = currentFont.Italic ? FontStyle.Regular : FontStyle.Italic;
            rtbJsonPreview.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
        }

        private void TsbtnUnderline_Click(object sender, EventArgs e)
        {
            var currentFont = rtbJsonPreview.SelectionFont;
            var newFontStyle = currentFont.Underline ? FontStyle.Regular : FontStyle.Underline;
            rtbJsonPreview.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
        }

        private void TsbtnCut_Click(object sender, EventArgs e)
        {
            rtbJsonPreview.Cut();
        }

        private void TsbtnCopy_Click(object sender, EventArgs e)
        {
            rtbJsonPreview.Copy();
        }

        private void TsbtnPaste_Click(object sender, EventArgs e)
        {
            rtbJsonPreview.Paste();
        }

        private void TsbtnUndo_Click(object sender, EventArgs e)
        {
            if (rtbJsonPreview.CanUndo)
            {
                rtbJsonPreview.Undo();
            }
        }

        private void TsbtnRedo_Click(object sender, EventArgs e)
        {
            if (rtbJsonPreview.CanRedo)
            {
                rtbJsonPreview.Redo();
            }
        }

        private void TsbtnClear_Click(object sender, EventArgs e)
        {
            rtbJsonPreview.Clear();
            rtbNumbers.Clear();
        }

        private void SetupForm()
        {
            // Wire up the event handlers
            cmbConditionType.SelectedIndexChanged += CmbConditionType_SelectedIndexChanged;
            CmbCounterType.SelectedIndexChanged += CmbCounterType_SelectedIndexChanged;
            txtQuestName.TextChanged += ControlValueChanged;
            txtQuest.TextChanged += ControlValueChanged;
            txt_ID.TextChanged += ControlValueChanged;
            chkCanShowNotificationsInGame.CheckedChanged += ControlValueChanged;
            txtDogtagLevel.TextChanged += ControlValueChanged;
            txtGlobalQuestCounterId.TextChanged += ControlValueChanged;
            txtIndex.TextChanged += ControlValueChanged;
            txtMaxDurability.TextChanged += ControlValueChanged;
            txtMinDurability.TextChanged += ControlValueChanged;
            chkOnlyFoundInRaid.CheckedChanged += ControlValueChanged;
            txtParentId.TextChanged += ControlValueChanged;
            txtTarget.TextChanged += ControlValueChanged;
            txtValue.TextChanged += ControlValueChanged;
            chkDynamicLocale.CheckedChanged += ControlValueChanged;
            txtID.TextChanged += ControlValueChanged;
            txtDtagFItem.TextChanged += ControlValueChanged;
            chkBoxDlFItem.CheckedChanged += ControlValueChanged;
            txtGqcIdFItem.TextChanged += ControlValueChanged;
            txtIdFItem.TextChanged += ControlValueChanged;
            txtIndexFItem.TextChanged += ControlValueChanged;
            txtMaxDurabilityFitem.TextChanged += ControlValueChanged;
            txtMinDurabilityFitem.TextChanged += ControlValueChanged;
            chkOnlyFoundInRaidFitem.CheckedChanged += ControlValueChanged;
            txtParentIdFitem.TextChanged += ControlValueChanged;
            txtTargetFitem.TextChanged += ControlValueChanged;
            txtValueFitem.TextChanged += ControlValueChanged;
            txtVisibleConditionsFitem.TextChanged += ControlValueChanged;
            txtVisibleConditions.TextChanged += ControlValueChanged;
            chkDynamicLocaleSkill.CheckedChanged += ControlValueChanged;
            txtGlobalQuestCounterIdSkill.TextChanged += ControlValueChanged;
            txtIDSkill.TextChanged += ControlValueChanged;
            txtIndexSkill.TextChanged += ControlValueChanged;
            txtParentIdSkill.TextChanged += ControlValueChanged;
            txtTargetSkill.TextChanged += ControlValueChanged;
            txtValueSkill.TextChanged += ControlValueChanged;
            txtVisibleConditionsSkill.TextChanged += ControlValueChanged;
            chkDynamicLocaleLIAL.CheckedChanged += ControlValueChanged;
            txtGlobalQuestCounterIdLIAL.TextChanged += ControlValueChanged;
            txtIDLIAL.TextChanged += ControlValueChanged;
            txtIndexLIAL.TextChanged += ControlValueChanged;
            txtParentIdLIAL.TextChanged += ControlValueChanged;
            txtTargetLIAL.TextChanged += ControlValueChanged;
            txtValueLIAL.TextChanged += ControlValueChanged;
            txtVisibleConditionsLIAL.TextChanged += ControlValueChanged;
            txtZoneIdLIAL.TextChanged += ControlValueChanged;
            chkDynamicLocalePB.CheckedChanged += ControlValueChanged;
            txtGlobalQuestCounterIdPB.TextChanged += ControlValueChanged;
            txtIDPB.TextChanged += ControlValueChanged;
            txtIndexPB.TextChanged += ControlValueChanged;
            txtParentIdPB.TextChanged += ControlValueChanged;
            txtParentIdPB.TextChanged += ControlValueChanged;
            txtTargetPB.TextChanged += ControlValueChanged;
            txtValuePB.TextChanged += ControlValueChanged;
            txtVisibleConditionsPB.TextChanged += ControlValueChanged;
            txtZoneIdPB.TextChanged += ControlValueChanged;
            CBsavRole.CheckedChanged += CBsavRole_CheckedChanged;
            CBbodyPart.CheckedChanged += CBbodyPart_CheckedChanged;

            btnAdd.Click += BtnAdd_Click;
            btnRemove.Click += BtnRemove_Click;

            rtbJsonPreview.VScroll += RtbJsonPreview_VScroll;
        }
    }
}
