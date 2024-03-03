using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatHut { 
    public static class EditorSharedData
    {
        private static RawMasterData _RawMasterData = null;
        public static RawMasterData RawMasterData
        {
            get
            {
                //TODO
                if (_RawMasterData == null)
                {
                    _RawMasterData = new RawMasterData(MasterDataEditorConfig.settings.CsvMasterDataPathList);

                    return _RawMasterData;
                }

                return _RawMasterData;
            }
        }

        // èâä˙âª
        static EditorSharedData()
        {

        }

        public static void UpdateData()
        {
            _RawMasterData = new RawMasterData(MasterDataEditorConfig.settings.CsvMasterDataPathList);
        }
    }
}