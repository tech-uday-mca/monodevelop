
ADDIN_BUILD = $(top_builddir)/build/AddIns/MonoDevelop.Refactoring
ASSEMBLY = $(ADDIN_BUILD)/MonoDevelop.Refactoring.dll

DEPS =  \
	$(top_builddir)/build/AddIns/MonoDevelop.DesignerSupport/MonoDevelop.DesignerSupport.dll \
	$(top_builddir)/build/bin/Mono.TextEditor.dll \
	$(top_builddir)/build/bin/MonoDevelop.Core.dll \
	$(top_builddir)/build/bin/MonoDevelop.Ide.dll \
	$(top_builddir)/build/bin/NRefactory.dll

REFS =  \
	$(GLIB_SHARP_LIBS) \
	$(GTK_SHARP_LIBS) \
	$(MONO_ADDINS_LIBS) \
	-r:Mono.Posix \
	-r:System \
	-r:System.Core \
	-r:System.Xml

FILES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/MonoDevelop.CodeGeneration.GenerateCodeWindow.cs \
	gtk-gui/MonoDevelop.Refactoring.EncapsulateFieldDialog.cs \
	gtk-gui/MonoDevelop.Refactoring.IntroduceConstant.IntroduceConstantDialog.cs \
	gtk-gui/MonoDevelop.Refactoring.OverridesImplementsDialog.cs \
	gtk-gui/MonoDevelop.Refactoring.RefactoringPreviewDialog.cs \
	gtk-gui/MonoDevelop.Refactoring.Rename.RenameItemDialog.cs \
	MonoDevelop.CodeGeneration/AbstractGenerateAction.cs \
	MonoDevelop.CodeGeneration/CodeGenerationCommands.cs \
	MonoDevelop.CodeGeneration/CodeGenerationOptions.cs \
	MonoDevelop.CodeGeneration/CreateConstructorGenerator.cs \
	MonoDevelop.CodeGeneration/EqualityMembersGenerator.cs \
	MonoDevelop.CodeGeneration/GenerateCodeWindow.cs \
	MonoDevelop.CodeGeneration/ICodeGenerator.cs \
	MonoDevelop.CodeGeneration/NullCheckGenerator.cs \
	MonoDevelop.CodeGeneration/OverrideMembersGenerator.cs \
	MonoDevelop.CodeGeneration/PropertyGenerator.cs \
	MonoDevelop.CodeGeneration/RaiseEventMethodGenerator.cs \
	MonoDevelop.CodeGeneration/ReadonlyPropertyGenerator.cs \
	MonoDevelop.CodeGeneration/ToStringGenerator.cs \
	MonoDevelop.CodeGeneration/WriteLineGenerator.cs \
	MonoDevelop.Refactoring.ConvertPropery/CreateBackingStore.cs \
	MonoDevelop.Refactoring.ConvertPropery/PropertyVisitor.cs \
	MonoDevelop.Refactoring.ConvertPropery/RemoveBackingStore.cs \
	MonoDevelop.Refactoring.CreateClass/CreateClassCodeGenerator.cs \
	MonoDevelop.Refactoring.CreateClass/CreateStructCodeGenerator.cs \
	MonoDevelop.Refactoring.DeclareLocal/DeclareLocalCodeGenerator.cs \
	MonoDevelop.Refactoring.DeclareLocal/DeclareLocalHandler.cs \
	MonoDevelop.Refactoring.DeclareLocal/GetContainingEmbeddedStatementVisitor.cs \
	MonoDevelop.Refactoring.ImplementInterface/ImplementAbstractMembers.cs \
	MonoDevelop.Refactoring.ImplementInterface/ImplementExplicit.cs \
	MonoDevelop.Refactoring.ImplementInterface/ImplementImplicit.cs \
	MonoDevelop.Refactoring.IntegrateTemporaryVariable/IntegrateTemporaryVariableHandler.cs \
	MonoDevelop.Refactoring.IntegrateTemporaryVariable/IntegrateTemporaryVariableRefactoring.cs \
	MonoDevelop.Refactoring.IntroduceConstant/IntroduceConstantDialog.cs \
	MonoDevelop.Refactoring.IntroduceConstant/IntroduceConstantHandler.cs \
	MonoDevelop.Refactoring.IntroduceConstant/IntroduceConstantRefactoring.cs \
	MonoDevelop.Refactoring.IntroduceFormatItem/IntroduceFormatItemRefactoring.cs \
	MonoDevelop.Refactoring.MoveTypeToFile/MoveTypeToFileRefactoring.cs \
	MonoDevelop.Refactoring.MoveTypeToFile/TypeFilterTransformer.cs \
	MonoDevelop.Refactoring.MoveTypeToFile/TypeSearchVisitor.cs \
	MonoDevelop.Refactoring.RefactorImports/FindTypeReferencesVisitor.cs \
	MonoDevelop.Refactoring.RefactorImports/RefactorImportsHandler.cs \
	MonoDevelop.Refactoring.RefactorImports/RemoveUnusedImportsRefactoring.cs \
	MonoDevelop.Refactoring.RefactorImports/SortImportsRefactoring.cs \
	MonoDevelop.Refactoring.Rename/RenameHandler.cs \
	MonoDevelop.Refactoring.Rename/RenameItemDialog.cs \
	MonoDevelop.Refactoring.Rename/RenameRefactoring.cs \
	MonoDevelop.Refactoring.Rename/RenameTextEditorExtension.cs \
	MonoDevelop.Refactoring/AbstractRefactoringCommandHandler.cs \
	MonoDevelop.Refactoring/Change.cs \
	MonoDevelop.Refactoring/EncapsulateFieldDialog.cs \
	MonoDevelop.Refactoring/FindDerivedClassesHandler.cs \
	MonoDevelop.Refactoring/FindReferencesHandler.cs \
	MonoDevelop.Refactoring/GotoDeclarationHandler.cs \
	MonoDevelop.Refactoring/HelperMethods.cs \
	MonoDevelop.Refactoring/ImportSymbolHandler.cs \
	MonoDevelop.Refactoring/INRefactoryASTProvider.cs \
	MonoDevelop.Refactoring/OverridesImplementsDialog.cs \
	MonoDevelop.Refactoring/QuickFixHandler.cs \
	MonoDevelop.Refactoring/RefactoringOperation.cs \
	MonoDevelop.Refactoring/RefactoringOptions.cs \
	MonoDevelop.Refactoring/RefactoringPreviewDialog.cs \
	MonoDevelop.Refactoring/RefactoringService.cs \
	MonoDevelop.Refactoring/RefactoryCommands.cs \
	MonoDevelop.Refactoring/ReferencesFinder.cs 

RES =  \
	gtk-gui/gui.stetic \
	MonoDevelop.Refactoring.addin.xml 

all: $(ASSEMBLY) $(ASSEMBLY).mdb

$(ADDIN_BUILD)/%: $(srcdir)/lib/%
	mkdir -p $(ADDIN_BUILD)
	cp $< $@

$(ASSEMBLY): $(build_sources) $(build_resources) $(DEPS)
	mkdir -p $(ADDIN_BUILD)
	$(CSC) $(CSC_FLAGS) $(REFS) $(build_deps) $(build_resources:%=/resource:%) $(build_sources) -out:$@ -target:library

$(ASSEMBLY).mdb: $(ASSEMBLY)

assemblydir = $(MD_ADDIN_DIR)/MonoDevelop.Refactoring
assembly_DATA = $(ASSEMBLY) $(ASSEMBLY).mdb

CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb
EXTRA_DIST = $(FILES) $(RES)

include $(top_srcdir)/Makefile.include
