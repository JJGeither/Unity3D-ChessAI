# Unity Chess AI

## Overview
This project is a chess environment built in Unity that lets users play chess against an AI opponent. The AI uses the Minimax algorithm with Alpha-Beta pruning to evaluate and make moves. The goal is to provide a challenging and efficient chess-playing experience.

## Features
- **Interactive Chess Environment**: Play against an AI or another player.
- **AI Opponent**: The AI evaluates positions using Minimax with Alpha-Beta pruning.
- **Configurable Depth**: Adjust AI difficulty by changing the search depth.
- **Legal Move Generation**: The AI only considers legal chess moves.
- **Performance Optimizations**: Efficient pruning improves response time.

## Technical Details

### Minimax Algorithm
This project uses the Minimax algorithm to evaluate future moves and select the best one. 

- The algorithm explores possible game states recursively.
- The AI assumes the opponent plays optimally and counters accordingly.
- Each position receives a numerical evaluation based on piece values and board control.

### Alpha-Beta Pruning
This project improves efficiency with Alpha-Beta pruning, which reduces the number of nodes evaluated in the Minimax tree. 

- Alpha (α) represents the best move found for the maximizer.
- Beta (β) represents the best move found for the minimizer.
- If a branch proves worse than a previously examined option, it is discarded.
- This reduces computation time and allows deeper searches.

### Depth Configuration
The AI's difficulty changes based on search depth:

- **Low Depth (1-2 moves ahead)**: Fast but weak AI.
- **Medium Depth (3-4 moves ahead)**: Balanced performance.
- **High Depth (5+ moves ahead)**: Stronger AI but slower move calculation.

### Board Evaluation Function
The AI evaluates the board using a heuristic function based on:
- **Material Advantage**: Sum of piece values (Pawn = 1, Knight/Bishop = 3, Rook = 5, Queen = 9).
- **Piece Positioning**: Encourages central control and king safety.
- **Mobility**: Rewards positions where more legal moves are available.
- **Checkmate Detection**: Identifies forced wins or losses.

