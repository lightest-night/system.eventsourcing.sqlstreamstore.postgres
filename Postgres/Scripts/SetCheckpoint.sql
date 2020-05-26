IF EXISTS ( SELECT checkpoint_id FROM __schema__.checkpoints WHERE checkpoint_id = @CheckpointId )
    UPDATE __schema__.checkpoints
    SET checkpoint = @Checkpoint
    WHERE checkpoint_id = @CheckpointId;
ELSE
    INSERT INTO __schema__.checkpoints (checkpoint_id, checkpoint)
    VALUES (@CheckpointId, @Checkpoint);