# Declarative AI
Introduces declarative self-improvement APIs into Microsoft.Extensions.AI

The goal here is to create declarative self-improving prompts that allow for self-improvement / optimization over time.
This borrows concepts from Standford's work around DSP through [DSPy](https://dspy.ai/) and Microsoft's [Trace](https://github.com/microsoft/Trace), and [OPRO](https://arxiv.org/pdf/[2309.03409](https://arxiv.org/pdf/2309.03409)).

## Parameters Dictionary

The dictionary should contain a string, ParameterDeclaration pair.

### ParameterDeclaration

This should be a record type that contains a string, object Metadata paired dictionary.
It should also contain a "Required", "Description", a "Type", and a "Name" property.
Each of the fields should be added to the metadata dictionary.

## Declarative AIContent

There should be a type that inherits from or is convertable to AIContent called "DeclarativeAIContent" that declares a "Parameters" dictionary.

## Declarative ChatMessage

There should be a type that inherits from or is convertable to ChatMessage called "DeclarativeChatMessage" that takes in a params style readonly collection.
The type should act as an abstraction over OPTO/OPRO implementations of prompts. Allowing DSPy, OPRO, or Trace to be provided as implementations for the pipeline.
It should also be usable for prompt templates.

The prompt type should support multi-modal inputs through the AIContents.

## Declarative Prompt

This type should inherit from AITool to provide a way to make prompts/templates executable as llm tools. It should take in a params style readonly collection of chatmessages.

## Declarative Engine

This type should take in options and have a "Generate" method that converts a declarative type to a non-declarative type that takes in a "BindingContext".

### Binding Context

The binding context should take in all in-context information provided to the llm for execution - including execution settings.

## Prompt Optimizer

This type should emulate the Microsoft "Trace" optimizer types, allowing for OPRO, Trace, etc. implementations to be substitute.
It should allow for feedback and application of feedback to mutate parameters.
