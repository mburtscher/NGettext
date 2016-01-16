﻿using System;
using NGettext.Plural.Ast;

namespace NGettext.Plural
{
	/// <summary>
	/// Represents a plural rule that will evaluate a given number 
	/// using an abstract syntax tree generated by a plural rule formula parser.
	/// </summary>
	public class AstPluralRule : IPluralRule
	{
		/// <summary>
		/// Maximum number of plural forms supported.
		/// </summary>
		public int NumPlurals { get; protected set; }

		protected Token AstRoot { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Token"/> class with given NumPlurals and abstract syntax tree.
		/// </summary>
		/// <param name="numPlurals"></param>
		/// <param name="astRoot">Abstract syntax tree root.</param>
		public AstPluralRule(int numPlurals, Token astRoot)
		{
			if (numPlurals <= 0)
				throw new ArgumentOutOfRangeException("numPlurals");
			if (astRoot == null)
				throw new ArgumentNullException("astRoot");

			this.NumPlurals = numPlurals;
			this.AstRoot = astRoot;
		}

		/// <summary>
		/// Evaluates a number and returns a plural form index.
		/// </summary>
		/// <param name="number">Number which needs to be evaluated.</param>
		/// <returns>Plural form index.</returns>
		public int Evaluate(long number)
		{
			return (int)this.Evaluate(this.AstRoot, number);
		}

		protected long Evaluate(Token node, long number)
		{
			switch (node.Type)
			{
				case TokenType.Number:
					return node.Value;

				case TokenType.N:
					return number;

				case TokenType.Plus:
					return this.Evaluate(node.Children[0], number)
						 + this.Evaluate(node.Children[1], number);

				case TokenType.Minus:
					return this.Evaluate(node.Children[0], number)
						 - this.Evaluate(node.Children[1], number);

				case TokenType.Divide:
					return this.Evaluate(node.Children[0], number)
						 / this.Evaluate(node.Children[1], number);

				case TokenType.Multiply:
					return this.Evaluate(node.Children[0], number)
						 * this.Evaluate(node.Children[1], number);

				case TokenType.Modulo:
					return this.Evaluate(node.Children[0], number)
						 % this.Evaluate(node.Children[1], number);

				case TokenType.GreaterThan:
					return this.Evaluate(node.Children[0], number)
						 > this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.GreaterOrEquals:
					return this.Evaluate(node.Children[0], number)
						>= this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.LessThan:
					return this.Evaluate(node.Children[0], number)
						 < this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.LessOrEquals:
					return this.Evaluate(node.Children[0], number)
						<= this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.Equals:
					return this.Evaluate(node.Children[0], number)
						== this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.NotEquals:
					return this.Evaluate(node.Children[0], number)
						!= this.Evaluate(node.Children[1], number)
						 ? 1 : 0;

				case TokenType.And:
					return this.Evaluate(node.Children[0], number) != 0
						&& this.Evaluate(node.Children[1], number) != 0
						 ? 1 : 0;

				case TokenType.Or:
					return this.Evaluate(node.Children[0], number) != 0
						|| this.Evaluate(node.Children[1], number) != 0
						 ? 1 : 0;

				case TokenType.Not:
					return this.Evaluate(node.Children[0], number) == 0
						 ? 1 : 0;

				case TokenType.TernaryIf:
					return this.Evaluate(node.Children[0], number) != 0
						 ? this.Evaluate(node.Children[1], number)
						 : this.Evaluate(node.Children[2], number);

				default:
					throw new InvalidOperationException(String.Format("Can not evaluate token: {0}.", node.Type));
			}
		}
	}
}
