// Copyright 2013 Joshua R. Rodgers
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlphaMapper.Renderer;

namespace AphaMapper
{
    public partial class AlphaMapper : Form
    {
        private RenderSystem _renderSystem;

        public AlphaMapper()
        {
            InitializeComponent();
        }

        private void OnShown(object sender, EventArgs e)
        {
            _renderSystem = new RenderSystem(m_RenderPanel, Directory.GetCurrentDirectory());
            _renderSystem.DrawShadowFrame += OnDrawShadowFrame;
            _renderSystem.DrawFrame += DrawFrame;
            _renderSystem.DrawSingleFrame();
        }

        private void DrawFrame()
        {
            
        }

        void OnDrawShadowFrame()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _renderSystem.SaveFrameToFile("C:\\Users\\Byte\\Desktop\\test.png");
        }
    }
}
