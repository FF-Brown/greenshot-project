﻿using System.Drawing;
using Dapplo.Windows.Common.Structs;
using Greenshot.Editor.Drawing;
using NUnit.Framework;

namespace Greenshot.Editor.Test
{
    [TestFixture]
    public class DrawableContainerListTest
    {
        [Test]
        [TestCase(10, 0)]
        [TestCase(-10, 0)]
        [TestCase(10, -10)]
        [TestCase(101, 0)]
        [TestCase(10, 110)]
        [TestCase(-10, -10)]
        [TestCase(110, 110)]
        public void SnapHelper_Left(int left, int top)
        {
            int width = 10;
            int height = 10;

            NativeRect bounds = new NativeRect(left, top, width, height);
            Size surfaceSize = new Size(100, 100);

            var newBounds = DrawableContainerList.SnapHelper(Direction.LEFT, bounds, surfaceSize);
            Assert.That(newBounds.Left, Is.Zero);
            Assert.That(newBounds.Top, Is.EqualTo(top));
            Assert.That(newBounds.Right, Is.EqualTo(newBounds.Left + width));
            Assert.That(newBounds.Bottom, Is.EqualTo(newBounds.Top + height));
        }

        [Test]
        [TestCase(10, 0)]
        [TestCase(-10, 0)]
        [TestCase(10, -10)]
        [TestCase(101, 0)]
        [TestCase(10, 110)]
        [TestCase(-10, -10)]
        [TestCase(110, 110)]
        public void SnapHelper_Right(int left, int top)
        {
            int width = 10;
            int height = 10;
            int surfaceWidth;
            int surfaceHeight = surfaceWidth = 100;

            NativeRect bounds = new NativeRect(left, top, width, height);
            Size surfaceSize = new Size(surfaceWidth, surfaceHeight);

            var newBounds = DrawableContainerList.SnapHelper(Direction.RIGHT, bounds, surfaceSize);
            Assert.That(newBounds.Left, Is.EqualTo(newBounds.Right - width));
            Assert.That(newBounds.Top, Is.EqualTo(top));
            Assert.That(newBounds.Right, Is.EqualTo(surfaceHeight));
            Assert.That(newBounds.Bottom, Is.EqualTo(newBounds.Top + height));
        }

        [Test]
        [TestCase(10, 0)]
        [TestCase(-10, 0)]
        [TestCase(10, -10)]
        [TestCase(101, 0)]
        [TestCase(10, 110)]
        [TestCase(-10, -10)]
        [TestCase(110, 110)]
        public void SnapHelper_Top(int left, int top)
        {
            int width = 10;
            int height = 10;
            int surfaceWidth;
            int surfaceHeight = surfaceWidth = 100;

            NativeRect bounds = new NativeRect(left, top, width, height);
            Size surfaceSize = new Size(surfaceWidth, surfaceHeight);

            var newBounds = DrawableContainerList.SnapHelper(Direction.TOP, bounds, surfaceSize);
            Assert.That(newBounds.Left, Is.EqualTo(left));
            Assert.That(newBounds.Top, Is.Zero);
            Assert.That(newBounds.Right, Is.EqualTo(newBounds.Left + width));
            Assert.That(newBounds.Bottom, Is.EqualTo(height));
        }

        [Test]
        [TestCase(10, 0)]
        [TestCase(-10, 0)]
        [TestCase(10, -10)]
        [TestCase(101, 0)]
        [TestCase(10, 110)]
        [TestCase(-10, -10)]
        [TestCase(110, 110)]
        public void SnapHelper_Bottom(int left, int top)
        {
            int width = 10;
            int height = 10;
            int surfaceWidth;
            int surfaceHeight = surfaceWidth = 100;

            NativeRect bounds = new NativeRect(left, top, width, height);
            Size surfaceSize = new Size(surfaceWidth, surfaceHeight);

            var newBounds = DrawableContainerList.SnapHelper(Direction.BOTTOM, bounds, surfaceSize);
            Assert.That(newBounds.Left, Is.EqualTo(left));
            Assert.That(newBounds.Top, Is.EqualTo(newBounds.Bottom - height));
            Assert.That(newBounds.Right, Is.EqualTo(newBounds.Left + width));
            Assert.That(newBounds.Bottom, Is.EqualTo(surfaceHeight));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 0)]
        [TestCase(0, 10)]
        [TestCase(10, 10)]
        public void GetExpansionFromSize_Left(int width, int height)
        {
            var expansion = DrawableContainerList.GetExpansionFromSize(Direction.LEFT, new Size(width, height));
            Assert.That(expansion.Left, Is.EqualTo(width));
            Assert.That(expansion.Right, Is.Zero);
            Assert.That(expansion.Top, Is.Zero);
            Assert.That(expansion.Bottom, Is.Zero);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 0)]
        [TestCase(0, 10)]
        [TestCase(10, 10)]
        public void GetExpansionFromSize_Right(int width, int height)
        {
            var expansion = DrawableContainerList.GetExpansionFromSize(Direction.RIGHT, new Size(width, height));
            Assert.That(expansion.Left, Is.Zero);
            Assert.That(expansion.Right, Is.EqualTo(width));
            Assert.That(expansion.Top, Is.Zero);
            Assert.That(expansion.Bottom, Is.Zero);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 0)]
        [TestCase(0, 10)]
        [TestCase(10, 10)]
        public void GetExpansionFromSize_Top(int width, int height)
        {
            var expansion = DrawableContainerList.GetExpansionFromSize(Direction.TOP, new Size(width, height));
            Assert.That(expansion.Left, Is.Zero);
            Assert.That(expansion.Right, Is.Zero);
            Assert.That(expansion.Top, Is.EqualTo(height));
            Assert.That(expansion.Bottom, Is.Zero);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 0)]
        [TestCase(0, 10)]
        [TestCase(10, 10)]
        public void GetExpansionFromSize_Bottom(int width, int height)
        {
            var expansion = DrawableContainerList.GetExpansionFromSize(Direction.BOTTOM, new Size(width, height));
            Assert.That(expansion.Left, Is.Zero);
            Assert.That(expansion.Right, Is.Zero);
            Assert.That(expansion.Top, Is.Zero);
            Assert.That(expansion.Bottom, Is.EqualTo(height));
        }
    }
}
