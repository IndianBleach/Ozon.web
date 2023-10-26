namespace Storage.Api.Validations
{
    public static class ValidationRegistry
    {
        public static CreateStorageValidator CreateStorageValidator = new CreateStorageValidator();

        public static CreateStorageCellValidator CreateStorageCellValidator = new CreateStorageCellValidator();

        public static CreateStorageActionTypeValidator CreateActionTypeValidator = new CreateStorageActionTypeValidator();

    }
}
