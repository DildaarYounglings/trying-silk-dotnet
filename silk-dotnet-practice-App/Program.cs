using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace silk_dotnet_practice_App
{
    public static class Program
    {
        private static IWindow window;
        private static IInputContext input;
        private static GL Gl;
        private static uint program;


        //Vertex shaders are run on each vertex
        private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec3 vPos;
        layout (location = 1) in vec4 vCol;

        out vec4 outCol;
        
        void main()
        {
            outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        //Fragment shaders are run on each fragment/pixel of geometry.
        private static readonly string FragmentShaderSource = @"
        #version 338 core
        out vec4 FragColor;

        in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        public static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default;
            options.Title = "Silk.NET project";
            options.Size = new Vector2D<int>(1280,720);
            window = Window.Create(options);

            window.Load += OnWindowLoad;
            window.Update += OnWindowUpdate;
            window.Render += OnWindowRender;

            window.Run();
        }
        private static void OnWindowLoad()
        {
            input = window.CreateInput();
            Gl = window.CreateOpenGL();
            foreach(IMouse mouse in input.Mice)
            {
                mouse.Click += (cursor,button,pos) => 
                {
                    Console.WriteLine("I Clicked");
                };
            }

            Gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);

            // vertexShader
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            // fragmentShader
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(vshader);
            Gl.CompileShader(fshader);

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if( status == 0){
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
        }
        private static void OnWindowUpdate(double d)
        {
            // No OpenGL
            
        }
        private static unsafe void OnWindowRender(double d)
        {
            // Yes OpenGL
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            uint vertices = Gl.GenBuffer();
            uint colors = Gl.GenBuffer();
            uint indices = Gl.GenBuffer();
            float[] vertexArray = new float[]{
                -0.5f,-0.5f,0.0f,
                +0.5f,+0.5f,0.0f,
                0.0f,+0.5f,0.0f,
            };
            float[] colorArray = new float[]{
                1.0f,0.0f,0.0f,1.0f,
                0.0f,0.0f,1.0f,1.0f,
                0.0f,1.0f,0.0f,1.0f,
            };
            uint[] indexArray = new uint[]{0,1,2};

            Gl.BindBuffer(GLEnum.ArrayBuffer,vertices);
            Gl.BufferData(GLEnum.ArrayBuffer,(ReadOnlySpan<float>)vertexArray.AsSpan(),GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0,3,GLEnum.Float,false,0,null);
            Gl.EnableVertexAttribArray(0);

            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, GLEnum.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);

            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            Gl.UseProgram(program);

            Gl.DrawElements(GLEnum.Triangles, 3, GLEnum.UnsignedInt,null);

            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(vao);

            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);

        }
    }
}