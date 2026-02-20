import { useState, useCallback } from "react";

interface ConfirmDialogState {
  isOpen: boolean;
  title: string;
  message: string;
  onConfirm: () => void;
}

interface UseConfirmDialogReturn {
  dialogState: ConfirmDialogState;
  confirm: (title: string, message: string) => Promise<boolean>;
  closeDialog: () => void;
}

export function useConfirmDialog(): UseConfirmDialogReturn {
  const [dialogState, setDialogState] = useState<ConfirmDialogState>({
    isOpen: false,
    title: "",
    message: "",
    onConfirm: () => {},
  });

  const confirm = useCallback(
    (title: string, message: string): Promise<boolean> => {
      return new Promise((resolve) => {
        setDialogState({
          isOpen: true,
          title,
          message,
          onConfirm: () => {
            setDialogState((prev) => ({ ...prev, isOpen: false }));
            resolve(true);
          },
        });
      });
    },
    [],
  );

  const closeDialog = useCallback(() => {
    setDialogState((prev) => ({ ...prev, isOpen: false }));
  }, []);

  return {
    dialogState,
    confirm,
    closeDialog,
  };
}
